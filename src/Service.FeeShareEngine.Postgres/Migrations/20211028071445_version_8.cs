using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class version_8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeeShareAmountInUsd",
                schema: "feeshares",
                table: "fee_shares",
                newName: "FeeToTargetConversionRate");

            migrationBuilder.RenameColumn(
                name: "FeeShareAmount",
                schema: "feeshares",
                table: "fee_shares",
                newName: "FeeShareAmountInTargetAsset");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                schema: "feeshares",
                table: "share_statistics",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentTimestamp",
                schema: "feeshares",
                table: "share_statistics",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "FeeShareAmountInFeeAsset",
                schema: "feeshares",
                table: "fee_shares",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "FeeShareAsset",
                schema: "feeshares",
                table: "fee_shares",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.DropColumn(
                name: "PaymentTimestamp",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.DropColumn(
                name: "FeeShareAmountInFeeAsset",
                schema: "feeshares",
                table: "fee_shares");

            migrationBuilder.DropColumn(
                name: "FeeShareAsset",
                schema: "feeshares",
                table: "fee_shares");

            migrationBuilder.RenameColumn(
                name: "FeeToTargetConversionRate",
                schema: "feeshares",
                table: "fee_shares",
                newName: "FeeShareAmountInUsd");

            migrationBuilder.RenameColumn(
                name: "FeeShareAmountInTargetAsset",
                schema: "feeshares",
                table: "fee_shares",
                newName: "FeeShareAmount");
        }
    }
}
