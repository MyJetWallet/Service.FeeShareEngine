using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.FeeShareEngine.Domain.Models;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.Grpc;
using Service.FeeShareEngine.Grpc.Models;
using Service.FeeShareEngine.Postgres;

namespace Service.FeeShareEngine.Services
{
    public class FeesService : IFeesService
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public FeesService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<GetAllStatsResponse> GetAllStatsAsync(PaginationRequest request)
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var stats = string.IsNullOrWhiteSpace(request.SearchText)
                ? await ctx.ShareStatistics.Skip(request.Skip).Take(request.Take).ToListAsync()
                : await ctx.ShareStatistics
                    .Where(t => t.AssetId.Contains(request.SearchText) ||
                                t.SettlementOperationId.Contains(request.SearchText))
                    .Skip(request.Skip).Take(request.Take)
                    .ToListAsync();


            return new GetAllStatsResponse()
            {
                Stats = stats
            };
        }

        public async Task<GetAllFeePaymentsResponse> GetAllFeePaymentsAsync(PaginationRequest request)
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var payments = string.IsNullOrWhiteSpace(request.SearchText)
                ? await ctx.FeePayments.Skip(request.Skip).Take(request.Take).ToListAsync()
                : await ctx.FeePayments
                    .Where(t => t.AssetId.Contains(request.SearchText) ||
                                t.ReferrerClientId.Contains(request.SearchText) ||
                                t.PaymentOperationId.Contains(request.SearchText))
                    .Skip(request.Skip).Take(request.Take).ToListAsync();
            return new GetAllFeePaymentsResponse()
            {
                FeePayments = payments
            };
        }

        public async Task<GetAllFeeSharesResponse> GetAllFeeSharesAsync(PaginationRequest request)
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var shares = string.IsNullOrWhiteSpace(request.SearchText)
                ? await ctx.FeeShares.Skip(request.Skip).Take(request.Take).ToListAsync()
                : await ctx.FeeShares
                    .Where(t => t.ReferrerClientId.Contains(request.SearchText) ||
                                t.OperationId.Contains(request.SearchText) ||
                                t.ReferralClientId.Contains(request.SearchText))
                    .Skip(request.Skip).Take(request.Take).ToListAsync();
            
            return new GetAllFeeSharesResponse()
            {
                FeeShares = shares
            };        
        }

        public async Task<OperationResponse> RetryFailedFeePayments()
        {
            try
            {
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var payments = await ctx.FeeShares.Where(t=>t.Status == PaymentStatus.FailedToPay).ToListAsync();
                foreach (var payment in payments)
                {
                    payment.Status = PaymentStatus.New;
                }
                await ctx.UpsetAsync(payments);
                return new OperationResponse()
                {
                    IsSuccess = true
                };
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

        public async Task<OperationResponse> RetryFailedFeeSettlements()
        {
            try
            {
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var settlements = await ctx.ShareStatistics.Where(t=>t.Status == SettlementStatus.FailedToPay).ToListAsync();
                foreach (var entity in settlements)
                {
                    entity.Status = SettlementStatus.New;
                }
                await ctx.UpsetAsync(settlements);
                return new OperationResponse()
                {
                    IsSuccess = true
                };
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
    }
}