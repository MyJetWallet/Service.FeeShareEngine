using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.FeeShareEngine.Domain.Models;
using Service.FeeShareEngine.Domain.Models.Models;

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
        private const string FeeShareGroupsTableName = "fee_share_groups";

        public DbSet<FeeShareEntity> FeeShares { get; set; }
        public DbSet<FeePaymentEntity> FeePayments { get; set; }
        public DbSet<ReferralMapEntity> Referrals { get; set; }
        public DbSet<ShareStatEntity> ShareStatistics { get; set; }
        public DbSet<FeeShareGroup> FeeShareGroups { get; set; }
        
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
            SetFeeGroupsEntity(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void SetFeeShareEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeeShareEntity>().ToTable(FeeShareTableName);
            modelBuilder.Entity<FeeShareEntity>().HasKey(e => e.OperationId);
            modelBuilder.Entity<FeeShareEntity>().Property(e => e.OperationId).HasMaxLength(512);
            modelBuilder.Entity<FeeShareEntity>().Property(e => e.FeeShareAmountInTargetAsset);
            modelBuilder.Entity<FeeShareEntity>().Property(e => e.ReferrerClientId).HasMaxLength(256);
            modelBuilder.Entity<FeeShareEntity>().Property(e => e.TimeStamp);
            modelBuilder.Entity<FeeShareEntity>().HasIndex(e => new {e.ReferrerClientId, e.TimeStamp});
            modelBuilder.Entity<FeeShareEntity>().HasIndex(e => e.ReferrerClientId);
        }
        
        private void SetFeePaymentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeePaymentEntity>().ToTable(FeePaymentTableName);
            modelBuilder.Entity<FeePaymentEntity>().HasKey(e => new { e.ReferrerClientId, e.PeriodFrom, e.PeriodTo, e.AssetId});
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.Amount);
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.ReferrerClientId).HasMaxLength(256);
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.CalculationTimestamp);
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.PaymentTimestamp).HasDefaultValue(DateTime.MinValue);
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.PeriodFrom);
            modelBuilder.Entity<FeePaymentEntity>().Property(e => e.PeriodTo);
            
            modelBuilder.Entity<FeePaymentEntity>().HasIndex(e => e.ReferrerClientId);
            modelBuilder.Entity<FeePaymentEntity>().HasIndex(e => e.PeriodFrom);
            modelBuilder.Entity<FeePaymentEntity>().HasIndex(e => new {e.PeriodFrom, e.PeriodTo});
            modelBuilder.Entity<FeePaymentEntity>().HasIndex(e => e.Status);


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
            modelBuilder.Entity<ShareStatEntity>().HasKey(e => new {e.PeriodFrom, e.PeriodTo, e.AssetId});
            modelBuilder.Entity<ShareStatEntity>().Property(e => e.PeriodFrom);
            modelBuilder.Entity<ShareStatEntity>().Property(e => e.PeriodTo);
            modelBuilder.Entity<ShareStatEntity>().Property(e => e.CalculationTimestamp);
            modelBuilder.Entity<ShareStatEntity>().Property(e => e.Amount);
            modelBuilder.Entity<ShareStatEntity>().Property(e => e.PaymentTimestamp).HasDefaultValue(DateTime.MinValue);
        }

        private void SetFeeGroupsEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeeShareGroup>().ToTable(FeeShareGroupsTableName);
            modelBuilder.Entity<FeeShareGroup>().HasKey(e => e.GroupId);
        }

        
        public async Task<int> UpsetAsync(IEnumerable<FeeShareEntity> entities)
        {
            var result = await FeeShares.UpsertRange(entities).On(e => e.OperationId).AllowIdentityMatch().RunAsync();
            return result;
        }
        
        public async Task<int> UpsetAsync(IEnumerable<FeePaymentEntity> entities)
        {
            var result = await FeePayments.UpsertRange(entities)
                .On(e => new {e.ReferrerClientId, e.PeriodFrom, e.PeriodTo, e.AssetId})
                .AllowIdentityMatch()
                .RunAsync();
            return result;
        }
        
        public async Task<int> UpsetAsync(IEnumerable<ReferralMapEntity> entities)
        {
            var result = await Referrals.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
        
        public async Task<int> UpsetAsync(IEnumerable<FeeShareGroup> entities)
        {
            var result = await FeeShareGroups.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
        
        public async Task<int> UpsetAsync(IEnumerable<ShareStatEntity> entities)
        {
            var result = await ShareStatistics.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
        
        public async Task SumShares(DateTime periodFrom, DateTime periodTo, ILogger logger)
        {
            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, @"Scripts/", "SumFeeShares.sql");
                using var script =
                    new StreamReader(path);
                var scriptBody = await script.ReadToEndAsync();
                
                 var periodFromString = $"{periodFrom.Year}-{periodFrom.Month.ToString().PadLeft(2, '0')}-{periodFrom.Day.ToString().PadLeft(2, '0')}" +
                                      $" {periodFrom.Hour.ToString().PadLeft(2, '0')}:{periodFrom.Minute.ToString().PadLeft(2, '0')}:{periodFrom.Second.ToString().PadLeft(2, '0')}";
                 scriptBody = scriptBody.Replace("${PeriodFrom}", periodFromString);
                
                 var periodToString = $"{periodTo.Year}-{periodTo.Month.ToString().PadLeft(2, '0')}-{periodTo.Day.ToString().PadLeft(2, '0')}" +
                                    $" {periodTo.Hour.ToString().PadLeft(2, '0')}:{periodTo.Minute.ToString().PadLeft(2, '0')}:{periodTo.Second.ToString().PadLeft(2, '0')}";
                 
                 scriptBody = scriptBody.Replace("${PeriodTo}", periodToString);
                 
                logger.LogInformation($"ExecPaidAsync start with date from: {periodFromString} and date to: {periodToString}");
                await Database.ExecuteSqlRawAsync(scriptBody);
                logger.LogInformation($"ExecPaidAsync finish with date from: {periodFromString} and date to: {periodToString}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        public override void Dispose()
        {
            _activity?.Dispose();
            base.Dispose();
        }
    }
}
