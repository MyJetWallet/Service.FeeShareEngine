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
    [Migration("20211021144703_InitialCreation")]
    partial class InitialCreation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("balancehistory")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Service.FeeShareEngine.Postgres.Models.FeePaymentEntity", b =>
                {
                    b.Property<string>("ReferrerClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("PeriodFrom")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CalculationTimestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("PaymentOperationId")
                        .HasColumnType("text");

                    b.Property<DateTime>("PaymentTimestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("PeriodTo")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("ReferrerClientId", "PeriodFrom");

                    b.HasIndex("PeriodFrom");

                    b.HasIndex("ReferrerClientId");

                    b.ToTable("fee_payments");
                });

            modelBuilder.Entity("Service.FeeShareEngine.Postgres.Models.FeeShareEntity", b =>
                {
                    b.Property<string>("OperationId")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<decimal>("FeeAmount")
                        .HasColumnType("numeric");

                    b.Property<decimal>("FeeAmountInUsd")
                        .HasColumnType("numeric");

                    b.Property<string>("FeeAsset")
                        .HasColumnType("text");

                    b.Property<decimal>("FeeShareAmountInUsd")
                        .HasColumnType("numeric");

                    b.Property<string>("FeeTransferOperationId")
                        .HasColumnType("text");

                    b.Property<DateTime>("PaymentTimestamp")
                        .HasColumnType("timestamp without time zone");

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

            modelBuilder.Entity("Service.FeeShareEngine.Postgres.Models.ReferralMapEntity", b =>
                {
                    b.Property<string>("ClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("ReferrerClientId")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("ClientId");

                    b.ToTable("referral_map");
                });

            modelBuilder.Entity("Service.FeeShareEngine.Postgres.Models.ShareStatEntity", b =>
                {
                    b.Property<DateTime>("PeriodFrom")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CalculationTimestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("PeriodTo")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("PeriodFrom");

                    b.ToTable("share_statistics");
                });
#pragma warning restore 612, 618
        }
    }
}
