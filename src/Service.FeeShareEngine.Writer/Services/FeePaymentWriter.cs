using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using MyJetWallet.Sdk.ServiceBus;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.Grpc.Models;
using Service.FeeShareEngine.Domain.Models;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.Postgres;

namespace Service.FeeShareEngine.Writer.Services
{
    public class FeePaymentWriter : IStartable
    {
        private readonly ILogger<FeePaymentWriter> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly SharesPaymentService _sharesPaymentService;
        private readonly MyTaskTimer _timer;
        private readonly IServiceBusPublisher<FeePaymentEntity> _feePaymentPublisher;
        private DateTime _paidPeriodDate;
        private PeriodTypes _periodType;
        public FeePaymentWriter(
            ILogger<FeePaymentWriter> logger, 
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, SharesPaymentService sharesPaymentService, IServiceBusPublisher<FeePaymentEntity> feePaymentPublisher)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _sharesPaymentService = sharesPaymentService;
            _feePaymentPublisher = feePaymentPublisher;
            _timer = new MyTaskTimer(nameof(FeePaymentWriter), TimeSpan.FromMinutes(1), logger, DoTimer);
        }
        
        private async Task DoTimer()
        {
            await CalculateFeePayments();
            await ExecuteFeePayments();

            var (periodStart, periodEnd) = PeriodHelper.GetPeriod(DateTime.UtcNow, _periodType);

            if (_paidPeriodDate < periodEnd)
            {
                await ExecuteFeeSettlements(periodEnd);
                await UpdateLastPaidPeriod();
            }
        }

        private async Task UpdateLastPaidPeriod()
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var last = ctx.ShareStatistics.OrderByDescending(t => t.PeriodTo).AsEnumerable().FirstOrDefault(t=>t.Status == SettlementStatus.Settled);
            _paidPeriodDate = last?.PeriodTo ?? DateTime.MinValue;
        }
        
        private async Task CalculateFeePayments()
        {
            var (periodStart, periodEnd) = PeriodHelper.GetPeriod(DateTime.UtcNow, _periodType);
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            await ctx.SumShares(periodStart, periodEnd, _logger);
        }

        private async Task ExecuteFeeSettlements(DateTime periodEnd)
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var unsettled = await ctx.ShareStatistics.Where(t => t.Status == SettlementStatus.New && t.PeriodTo <= periodEnd).ToListAsync();
            foreach (var settle in unsettled)
            {
                await _sharesPaymentService.TransferSettlementPayment(settle);
            }
            await ctx.UpsetAsync(unsettled);
        }

        private async Task ExecuteFeePayments()
        {
            List<FeePaymentEntity> payments;
            do
            {
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var query =
                    from payment in ctx.FeePayments
                    where payment.Status == PaymentStatus.New
                    join stat in ctx.ShareStatistics
                        on new { payment.AssetId, payment.PeriodFrom, payment.PeriodTo } equals new
                            { stat.AssetId, stat.PeriodFrom, stat.PeriodTo }
                    where stat.Status == SettlementStatus.Settled
                    select payment;
                
                payments = await query.Take(100).ToListAsync();

                foreach (var payment in payments)
                {
                    await _sharesPaymentService.PayFeeToReferrers(payment);
                }

                await ctx.UpsetAsync(payments);
            } while (payments.Any());

        }

        public void Start()
        {
            _timer.Start();
        }
    }
}


