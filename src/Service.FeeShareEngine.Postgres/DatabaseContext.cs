using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.FeeShareEngine.Domain.Models;
using Service.FeeShareEngine.Postgres.Models;

namespace Service.FeeShareEngine.Postgres
{
    public class DatabaseContext : DbContext
    {
        public static DatabaseContext Create(DbContextOptionsBuilder<DatabaseContext> options)
        {
            var activity = MyTelemetry.StartActivity($"Database context {Schema}")?.AddTag("db-schema", Schema);
            
            var ctx = new DatabaseContext(options.Options) {_activity = activity};
            return ctx;
        }

        public const string Schema = "feeshares";
        
        private const string ReferralMapTableName = "referral_map";
        private const string FeeShareTableName = "fee_shares";
        private const string FeePaymentTableName = "fee_payments";
        private const string ShareStatisticsTableName = "share_statistics";
        
        public DbSet<FeeShareEntity> FeeShares { get; set; }
        public DbSet<FeePaymentEntity> FeePayments { get; set; }
        public DbSet<ReferralMapEntity> Referrals { get; set; }
        public DbSet<ShareStatEntity> ShareStatistics { get; set; }
        
        private Activity _activity;

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        public static ILoggerFactory LoggerFactory { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
            
            SetFeeShareEntity(modelBuilder);
            SetFeePaymentEntity(modelBuilder);
            SetReferralMapEntity(modelBuilder);
            SetShareStatEntity(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void SetFeeShareEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeeShareEntity>().ToTable(FeeShareTableName);
            modelBuilder.Entity<FeeShareEntity>().HasKey(e => e.OperationId);
            modelBuilder.Entity<FeeShareEntity>().Property(e => e.OperationId).HasMaxLength(512);
            modelBuilder.Entity<FeeShareEntity>().Property(e => e.FeeShareAmountInUsd);
            modelBuilder.Entity<FeeShareEntity>().Property(e => e.ReferrerClientId).HasMaxLength(256);
            modelBuilder.Entity<FeeShareEntity>().Property(e => e.TimeStamp);
            modelBuilder.Entity<FeeShareEntity>().HasIndex(e => new {e.ReferrerClientId, e.TimeStamp});
            modelBuilder.Entity<FeeShareEntity>().HasIndex(e => e.ReferrerClientId);
        }
        
        private void SetFeePaymentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeePaymentEntity>().ToTable(FeePaymentTableName);
            modelBuilder.Entity<FeePaymentEntity>().HasKey(e => new { e.ReferrerClientId, e.PeriodFrom});
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.Amount);
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.ReferrerClientId).HasMaxLength(256);
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.CalculationTimestamp);
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.PaymentTimestamp);
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.PeriodFrom);
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.PeriodTo);
            modelBuilder.Entity<FeePaymentEntity>().HasIndex(e => e.ReferrerClientId);
            modelBuilder.Entity<FeePaymentEntity>().HasIndex(e => e.PeriodFrom);
        }

        private void SetReferralMapEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReferralMapEntity>().ToTable(ReferralMapTableName);
            modelBuilder.Entity<ReferralMapEntity>().HasKey(e => e.ClientId);
            modelBuilder.Entity<ReferralMapEntity>().Property(e => e.ClientId).HasMaxLength(256);
            modelBuilder.Entity<ReferralMapEntity>().Property(e => e.ReferrerClientId).HasMaxLength(256);
        }

        private void SetShareStatEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShareStatEntity>().ToTable(ShareStatisticsTableName);
            modelBuilder.Entity<ShareStatEntity>().HasKey(e => e.PeriodFrom);
            modelBuilder.Entity<ShareStatEntity>().Property(e => e.PeriodFrom);
            modelBuilder.Entity<ShareStatEntity>().Property(e => e.PeriodTo);
            modelBuilder.Entity<ShareStatEntity>().Property(e => e.CalculationTimestamp);
            modelBuilder.Entity<ShareStatEntity>().Property(e => e.Amount);
        }

        
        public async Task<int> UpsetAsync(IEnumerable<FeeShareEntity> entities)
        {
            var result = await FeeShares.UpsertRange(entities).On(e => e.OperationId).NoUpdate().RunAsync();
            return result;
        }
        
        public async Task<int> UpsetAsync(IEnumerable<FeePaymentEntity> entities)
        {
            var result = await FeePayments.UpsertRange(entities)
                .On(e => new {e.ReferrerClientId, e.PeriodFrom})
                .WhenMatched((oldEntity, newEntity) => oldEntity.Status == PaymentStatus.Paid ? oldEntity : newEntity)
                .RunAsync();
            return result;
        }
        
        public async Task<int> UpsetAsync(IEnumerable<ShareStatEntity> entities)
        {
            var result = await ShareStatistics.UpsertRange(entities)
                .On(e => e.PeriodFrom)
                .WhenMatched((oldEntity, newEntity) => newEntity.CalculationTimestamp > oldEntity.CalculationTimestamp ? newEntity : oldEntity)
                .RunAsync();
            return result;
        }
        
        public async Task<int> UpsetAsync(IEnumerable<ReferralMapEntity> entities)
        {
            var result = await Referrals.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }

        public override void Dispose()
        {
            _activity?.Dispose();
            base.Dispose();
        }
    }
}
