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

        public FeePaymentWriter(
            ILogger<FeePaymentWriter> logger, 
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, FeePaymentService feePaymentService, IServiceBusPublisher<FeePaymentEntity> feePaymentPublisher)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _feePaymentService = feePaymentService;
            _feePaymentPublisher = feePaymentPublisher;


            _timer = new MyTaskTimer(nameof(FeePaymentWriter), TimeSpan.FromHours(6), logger, DoTimer);
        }
        
        private async Task DoTimer()
        {
            await SumFeeShares();
            
            var today = DateTime.UtcNow;
            var endOfMonth = today.AddMonths(1).AddDays(-1);

            if (endOfMonth.Date == today.Date)
            {
                await ExecuteFeePayments();
                await RecordPaymentStatistics();
            }

        }

        private async Task SumFeeShares()
        {
            var today = DateTime.UtcNow;
            var periodFrom = new DateTime(today.Year, today.Month, 1);
            var periodTo = periodFrom.AddMonths(1).AddDays(-1);
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var shares = ctx.FeeShares.Where(t => t.TimeStamp >= periodFrom).AsEnumerable().GroupBy(t=>t.ReferrerClientId);
            var payments = new List<FeePaymentEntity>();
            foreach (var shareGroup in shares)
            {
                payments.Add(new FeePaymentEntity
                {
                    PaymentOperationId = Guid.NewGuid().ToString("N"),
                    ReferrerClientId = shareGroup.Key,
                    Amount = shareGroup.Sum(t => t.FeeShareAmountInUsd),
                    PeriodFrom = periodFrom,
                    PeriodTo = periodTo,
                    CalculationTimestamp = DateTime.UtcNow,
                    Status = PaymentStatus.New
                });
            }
            
            await ctx.UpsetAsync(payments);
            await ctx.SaveChangesAsync();
        }

        private async Task ExecuteFeePayments()
        {
            var currentPeriod = DateTime.UtcNow.AddMonths(-1);
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var payments = ctx.FeePayments.Where(t => t.PeriodTo < currentPeriod && t.Status == PaymentStatus.New)
                .ToList();

            foreach (var payment in payments)
            {

                var isSuccess = await _feePaymentService.PayFeeToReferrers(payment);
                payment.PaymentTimestamp = DateTime.UtcNow;
                payment.Status = isSuccess ? PaymentStatus.Paid : PaymentStatus.FailedToPay;
            }

            await ctx.UpsetAsync(payments);
            await ctx.SaveChangesAsync();

            await _feePaymentPublisher.PublishAsync(payments);
        }

        private async Task RecordPaymentStatistics()
        {
            var today = DateTime.UtcNow;
            var periodFrom = new DateTime(today.Year, today.Month, 1);
            var periodTo = periodFrom.AddMonths(1).AddDays(-1);
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var payments = ctx.FeePayments.Where(t => t.PeriodFrom >= periodFrom);
            var stat = new ShareStatEntity
            {
                Amount = payments.Sum(t => t.Amount),
                PeriodFrom = periodFrom,
                PeriodTo = periodTo,
                CalculationTimestamp = DateTime.UtcNow
            };
            await ctx.UpsetAsync(new[] { stat });
            await ctx.SaveChangesAsync();
        }

        public void Start()
        {
            _timer.Start();
        }
    }
}


