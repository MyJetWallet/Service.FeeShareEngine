using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.Grpc;
using Service.FeeShareEngine.Grpc.Models;
using Service.FeeShareEngine.NoSql;
using Service.FeeShareEngine.Postgres;
using Service.FeeShareEngine.Settings;

namespace Service.FeeShareEngine.Services
{
    public class FeeShareEngineManager: IFeeShareEngineManager
    {
        private readonly ILogger<FeeShareEngineManager> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<FeeShareSettingsNoSqlEntity> _settingWriter;
        private readonly IServiceBusPublisher<ReferralMapChangeMessage> _publisher;
        public FeeShareEngineManager(ILogger<FeeShareEngineManager> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IMyNoSqlServerDataWriter<FeeShareSettingsNoSqlEntity> settingWriter, IServiceBusPublisher<ReferralMapChangeMessage> publisher)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _settingWriter = settingWriter;
            _publisher = publisher;
        }

        public async Task<OperationResponse> AddReferralLink(AddReferralRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.ClientId) || string.IsNullOrWhiteSpace(request.ReferrerClientId))
                {
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "ClientIds can not be empty"
                    };
                }
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var feeShareGroup = string.IsNullOrWhiteSpace(request.FeeShareGroupId)
                    ? await ctx.FeeShareGroups.FirstOrDefaultAsync(t => t.IsDefault)
                    : await ctx.FeeShareGroups.FirstOrDefaultAsync(t => t.GroupId == request.FeeShareGroupId);

                if (feeShareGroup == null)
                {
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "FeeShareGroup Not Found"
                    };
                }
                await ctx.UpsetAsync(new[]
                {
                    new ReferralMapEntity()
                    {
                        ReferrerClientId = request.ReferrerClientId,
                        ClientId = request.ClientId,
                        FeeShareGroupId = request.FeeShareGroupId
                    }
                });
                return new OperationResponse() { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorCode = e.Message
                };
            }
        }

        public async Task<OperationResponse> DeleteReferralLink(DeleteReferralRequest request)
        {
            try
            {
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var entity = await ctx.Referrals.FirstOrDefaultAsync(t => t.ClientId == request.ClientId);
                if (entity != null)
                {
                    ctx.Referrals.Remove(entity);
                    await ctx.SaveChangesAsync();
                    await _publisher.PublishAsync(new ReferralMapChangeMessage()
                    {
                        ClientId = entity.ClientId
                    });
                }
                return new OperationResponse() { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorCode = e.Message
                };
            }
        }

        public async Task<GetAllReferralMapsResponse> GetAllReferralMaps(PaginationRequest request)
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var maps = string.IsNullOrWhiteSpace(request.SearchText)
                ? await ctx.Referrals.Skip(request.Skip).Take(request.Take)
                    .ToListAsync()
                : await ctx.Referrals
                    .Where(t => t.ClientId.Contains(request.SearchText) ||
                                t.ReferrerClientId.Contains(request.SearchText) ||
                                t.FeeShareGroupId.Contains(request.SearchText))
                    .Skip(request.Skip).Take(request.Take)
                    .ToListAsync();
            return new GetAllReferralMapsResponse()
            {
                Referrals = maps
            };
        }

        public async Task<AllFeeGroupsResponse> GetAllFeeShareGroups(PaginationRequest request)
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var groups = await ctx.FeeShareGroups.Skip(request.Skip).Take(request.Take).ToListAsync();
            return new AllFeeGroupsResponse
            {
                Groups = groups
            };
        }

        public async Task<OperationResponse> AddOrUpdateFeeShareGroup(FeeShareGroup request)
        {
            try
            {
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                await ctx.UpsetAsync(new[]
                {
                    request
                });
                await _publisher.PublishAsync(new ReferralMapChangeMessage()
                {
                    FeeShareGroupId = request.GroupId
                });
                return new OperationResponse() { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorCode = e.Message
                };
            }
        }

        public async Task<OperationResponse> DeleteFeeShareGroup(DeleteGroupRequest request)
        {
            try
            {
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var entity = await ctx.FeeShareGroups.FirstOrDefaultAsync(t => t.GroupId == request.FeeShareGroupId);
                var users = ctx.Referrals.Count(t => t.FeeShareGroupId == request.FeeShareGroupId);
                if (users > 0)
                {
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "Unable to delete fee share group that contains users"
                    };
                }
                if (entity != null)
                {
                    ctx.FeeShareGroups.Remove(entity);
                    await ctx.SaveChangesAsync();
                    await _publisher.PublishAsync(new ReferralMapChangeMessage()
                    {
                        FeeShareGroupId = entity.GroupId
                    });
                }

                return new OperationResponse() { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorCode = e.Message
                };
            }
        }

        public async Task<OperationResponse> UpdateFeeShareSettings(FeeShareSettingsModel request)
        {
            try
            {
                await _settingWriter.InsertOrReplaceAsync(FeeShareSettingsNoSqlEntity.Create(request));
                return new OperationResponse() { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorCode = e.Message
                };
            }
        }

        public async Task<FeeShareSettingsModel> GetFeeShareSettings()
        {
            var entity = await _settingWriter.GetAsync(FeeShareSettingsNoSqlEntity.GeneratePartitionKey(),
                FeeShareSettingsNoSqlEntity.GenerateRowKey());
            if (entity == null)
            {
                if(!Enum.TryParse(typeof(PeriodTypes), Program.Settings.PeriodType, true, out var type))
                {
                    _logger.LogError("Period {period} cannot be parsed", Program.Settings.PeriodType);
                    throw new Exception($"Period {Program.Settings.PeriodType} cannot be parsed");
                }
                var periodType = (PeriodTypes)type;
                var settings = new FeeShareSettingsModel
                {
                    FeeShareEngineWalletId = Program.Settings.ServiceWalletId,
                    FeeShareEngineClientId = Program.Settings.ServiceWalletClientId,
                    FeeShareEngineBrokerId = Program.Settings.ServiceWalletBrokerId,
                    FeeShareEngineBrandId = Program.Settings.ServiceWalletBrandId,
                    CurrentPeriod = periodType
                };
                
                await _settingWriter.InsertOrReplaceAsync(FeeShareSettingsNoSqlEntity.Create(settings));
                return settings;
            }

            return entity.Settings;
        }
    }
}
