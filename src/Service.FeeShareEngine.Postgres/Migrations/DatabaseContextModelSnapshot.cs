﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Service.FeeShareEngine.Postgres;

#nullable disable

namespace Service.FeeShareEngine.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("feeshares")
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Service.FeeShareEngine.Domain.Models.Models.FeePaymentEntity", b =>
                {
                    b.Property<string>("ReferrerClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("PeriodFrom")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<DateTime>("PeriodTo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("AssetId")
                        .HasColumnType("text");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CalculationTimestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastTs")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("PaymentOperationId")
                        .HasColumnType("text");

                    b.Property<DateTime>("PaymentTimestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("ReferrerWalletId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("ReferrerClientId", "PeriodFrom", "PeriodTo", "AssetId");

                    b.HasIndex("LastTs");

                    b.HasIndex("PeriodFrom");

                    b.HasIndex("ReferrerClientId");

                    b.HasIndex("Status");

                    b.HasIndex("PeriodFrom", "PeriodTo");

                    b.ToTable("fee_payments", "feeshares");
                });

            modelBuilder.Entity("Service.FeeShareEngine.Domain.Models.Models.FeeShareEntity", b =>
                {
                    b.Property<string>("OperationId")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<decimal>("FeeAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("FeeAsset")
                        .HasColumnType("text");

                    b.Property<decimal>("FeeShareAmountInFeeAsset")
                        .HasColumnType("numeric");

                    b.Property<decimal>("FeeShareAmountInTargetAsset")
                        .HasColumnType("numeric");

                    b.Property<string>("FeeShareAsset")
                        .HasColumnType("text");

                    b.Property<decimal>("FeeToTargetConversionRate")
                        .HasColumnType("numeric");

                    b.Property<string>("FeeTransferOperationId")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastTs")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("ReferralClientId")
                        .HasColumnType("text");

                    b.Property<string>("ReferrerClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("TimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.HasKey("OperationId");

                    b.HasIndex("LastTs");

                    b.HasIndex("ReferrerClientId");

                    b.HasIndex("ReferrerClientId", "TimeStamp");

                    b.ToTable("fee_shares", "feeshares");
                });

            modelBuilder.Entity("Service.FeeShareEngine.Domain.Models.Models.FeeShareGroup", b =>
                {
                    b.Property<string>("GroupId")
                        .HasColumnType("text");

                    b.Property<string>("AssetId")
                        .HasColumnType("text");

                    b.Property<decimal>("FeePercent")
                        .HasColumnType("numeric");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastTs")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.HasKey("GroupId");

                    b.HasIndex("LastTs");

                    b.ToTable("fee_share_groups", "feeshares");
                });

            modelBuilder.Entity("Service.FeeShareEngine.Domain.Models.Models.ReferralMapEntity", b =>
                {
                    b.Property<string>("ClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("FeeShareGroupId")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastTs")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("ReferrerClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("ClientId");

                    b.HasIndex("LastTs");

                    b.ToTable("referral_map", "feeshares");
                });

            modelBuilder.Entity("Service.FeeShareEngine.Domain.Models.Models.ShareStatEntity", b =>
                {
                    b.Property<DateTime>("PeriodFrom")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<DateTime>("PeriodTo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("AssetId")
                        .HasColumnType("text");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CalculationTimestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastTs")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<DateTime>("PaymentTimestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("SettlementOperationId")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("PeriodFrom", "PeriodTo", "AssetId");

                    b.HasIndex("LastTs");

                    b.ToTable("share_statistics", "feeshares");
                });
#pragma warning restore 612, 618
        }
    }
}
