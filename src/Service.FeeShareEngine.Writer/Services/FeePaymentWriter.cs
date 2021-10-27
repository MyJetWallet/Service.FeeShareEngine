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
        private readonly FeePaymentService _feePaymentService;
        private readonly MyTaskTimer _timer;
        private readonly IServiceBusPublisher<FeePaymentEntity> _feePaymentPublisher;
        private DateTime _paidPeriodDate;
        private PeriodTypes _periodType;
        public FeePaymentWriter(
            ILogger<FeePaymentWriter> logger, 
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, FeePaymentService feePaymentService, IServiceBusPublisher<FeePaymentEntity> feePaymentPublisher)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _feePaymentService = feePaymentService;
            _feePaymentPublisher = feePaymentPublisher;
            _timer = new MyTaskTimer(nameof(FeePaymentWriter), TimeSpan.FromMinutes(1), logger, DoTimer);
        }
        
        private async Task DoTimer()
        {
            await CalculateFeePayments();
            
            var (periodStart, periodEnd) = PeriodHelper.GetPeriod(DateTime.UtcNow, _periodType);

            if (_paidPeriodDate < periodEnd)
            {
                await ExecuteFeePayments(periodEnd);
                await UpdateLastPaidPeriod();
            }
        }

        private async Task UpdateLastPaidPeriod()
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var last = ctx.ShareStatistics.OrderByDescending(t => t.PeriodTo).AsEnumerable().FirstOrDefault();
            _paidPeriodDate = last?.PeriodTo ?? DateTime.MinValue;
        }
        
        private async Task CalculateFeePayments()
        {
            var (periodStart, periodEnd) = PeriodHelper.GetPeriod(DateTime.UtcNow, _periodType);
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            //await ctx.SumShares(periodStart, periodEnd, _logger);
        }

        private async Task ExecuteFeePayments(DateTime periodEnd)
        {
            List<FeePaymentEntity> payments;
            do
            {
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                payments = ctx.FeePayments
                    .Where(t => t.Status == PaymentStatus.New && t.PeriodTo <= periodEnd)
                    .Take(100)
                    .ToList();

                foreach (var payment in payments)
                {
                    await _feePaymentService.PayFeeToReferrers(payment);
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


