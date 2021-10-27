using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public FeeShareEngineManager(ILogger<FeeShareEngineManager> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IMyNoSqlServerDataWriter<FeeShareSettingsNoSqlEntity> settingWriter)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _settingWriter = settingWriter;
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
                var feeShareGroup =
                    await ctx.FeeShareGroups.FirstOrDefaultAsync(t => t.GroupId == request.FeeShareGroupId);

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
                        FeeShareGroup = feeShareGroup
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
                    ctx.Referrals.Remove(entity);
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

        public async Task<GetAllReferralMapsResponse> GetAllReferralMaps()
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var maps = await ctx.Referrals        
                .Include(t=>t.FeeShareGroup)
                .ToListAsync();
            return new GetAllReferralMapsResponse()
            {
                Referrals = maps
            };
        }

        public async Task<AllFeeGroupsResponse> GetAllFeeShareGroups()
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var groups = await ctx.FeeShareGroups.ToListAsync();
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
                if (entity != null)
                    ctx.FeeShareGroups.Remove(entity);
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
