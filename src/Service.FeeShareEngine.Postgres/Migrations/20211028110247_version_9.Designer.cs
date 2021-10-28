﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Service.FeeShareEngine.Postgres;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20211028110247_version_9")]
    partial class version_9
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("feeshares")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Service.FeeShareEngine.Domain.Models.Models.FeePaymentEntity", b =>
                {
                    b.Property<string>("ReferrerClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("PeriodFrom")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("PeriodTo")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("AssetId")
                        .HasColumnType("text");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CalculationTimestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text");

                    b.Property<string>("PaymentOperationId")
                        .HasColumnType("text");

                    b.Property<DateTime>("PaymentTimestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("ReferrerClientId", "PeriodFrom", "PeriodTo", "AssetId");

                    b.HasIndex("PeriodFrom");

                    b.HasIndex("ReferrerClientId");

                    b.HasIndex("Status");

                    b.HasIndex("PeriodFrom", "PeriodTo");

                    b.ToTable("fee_payments");
                });

            modelBuilder.Entity("Service.FeeShareEngine.Domain.Models.Models.FeeShareEntity", b =>
                {
                    b.Property<string>("OperationId")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("BrokerId")
                        .HasColumnType("text");

                    b.Property<string>("ConverterWalletId")
                        .HasColumnType("text");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text");

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

                    b.Property<string>("FeeShareWalletId")
                        .HasColumnType("text");

                    b.Property<decimal>("FeeToTargetConversionRate")
                        .HasColumnType("numeric");

                    b.Property<string>("FeeTransferOperationId")
                        .HasColumnType("text");

                    b.Property<DateTime>("PaymentTimestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ReferralClientId")
                        .HasColumnType("text");

                    b.Property<string>("ReferrerClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("OperationId");

                    b.HasIndex("ReferrerClientId");

                    b.HasIndex("ReferrerClientId", "TimeStamp");

                    b.ToTable("fee_shares");
                });

            modelBuilder.Entity("Service.FeeShareEngine.Domain.Models.Models.FeeShareGroup", b =>
                {
                    b.Property<string>("GroupId")
                        .HasColumnType("text");

                    b.Property<string>("AssetId")
                        .HasColumnType("text");

                    b.Property<decimal>("FeePercent")
                        .HasColumnType("numeric");

                    b.HasKey("GroupId");

                    b.ToTable("fee_share_groups");
                });

            modelBuilder.Entity("Service.FeeShareEngine.Domain.Models.Models.ReferralMapEntity", b =>
                {
                    b.Property<string>("ClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("FeeShareGroupId")
                        .HasColumnType("text");

                    b.Property<string>("ReferrerClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("ClientId");

                    b.ToTable("referral_map");
                });

            modelBuilder.Entity("Service.FeeShareEngine.Domain.Models.Models.ShareStatEntity", b =>
                {
                    b.Property<DateTime>("PeriodFrom")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("PeriodTo")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("AssetId")
                        .HasColumnType("text");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CalculationTimestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text");

                    b.Property<DateTime>("PaymentTimestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

                    b.Property<string>("SettlementOperationId")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("PeriodFrom", "PeriodTo", "AssetId");

                    b.ToTable("share_statistics");
                });
#pragma warning restore 612, 618
        }
    }
}
