using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class InitialCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "balancehistory");

            migrationBuilder.CreateTable(
                name: "fee_payments",
                schema: "balancehistory",
                columns: table => new
                {
                    ReferrerClientId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CalculationTimestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PaymentTimestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentOperationId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fee_payments", x => new { x.ReferrerClientId, x.PeriodFrom });
                });

            migrationBuilder.CreateTable(
                name: "fee_shares",
                schema: "balancehistory",
                columns: table => new
                {
                    OperationId = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ReferrerClientId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FeeShareAmountInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FeeTransferOperationId = table.Column<string>(type: "text", nullable: true),
                    FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    FeeAsset = table.Column<string>(type: "text", nullable: true),
                    FeeAmountInUsd = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentTimestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fee_shares", x => x.OperationId);
                });

            migrationBuilder.CreateTable(
                name: "referral_map",
                schema: "balancehistory",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ReferrerClientId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referral_map", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "share_statistics",
                schema: "balancehistory",
                columns: table => new
                {
                    PeriodFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CalculationTimestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_share_statistics", x => x.PeriodFrom);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fee_payments_PeriodFrom",
                schema: "balancehistory",
                table: "fee_payments",
                column: "PeriodFrom");

            migrationBuilder.CreateIndex(
                name: "IX_fee_payments_ReferrerClientId",
                schema: "balancehistory",
                table: "fee_payments",
                column: "ReferrerClientId");

            migrationBuilder.CreateIndex(
                name: "IX_fee_shares_ReferrerClientId",
                schema: "balancehistory",
                table: "fee_shares",
                column: "ReferrerClientId");

            migrationBuilder.CreateIndex(
                name: "IX_fee_shares_ReferrerClientId_TimeStamp",
                schema: "balancehistory",
                table: "fee_shares",
                columns: new[] { "ReferrerClientId", "TimeStamp" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fee_payments",
                schema: "balancehistory");

            migrationBuilder.DropTable(
                name: "fee_shares",
                schema: "balancehistory");

            migrationBuilder.DropTable(
                name: "referral_map",
                schema: "balancehistory");

            migrationBuilder.DropTable(
                name: "share_statistics",
                schema: "balancehistory");
        }
    }
}
