using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class AddLastTs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastTs",
                schema: "feeshares",
                table: "share_statistics",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTs",
                schema: "feeshares",
                table: "referral_map",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTs",
                schema: "feeshares",
                table: "fee_shares",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTs",
                schema: "feeshares",
                table: "fee_share_groups",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTs",
                schema: "feeshares",
                table: "fee_payments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTs",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.DropColumn(
                name: "LastTs",
                schema: "feeshares",
                table: "referral_map");

            migrationBuilder.DropColumn(
                name: "LastTs",
                schema: "feeshares",
                table: "fee_shares");

            migrationBuilder.DropColumn(
                name: "LastTs",
                schema: "feeshares",
                table: "fee_share_groups");

            migrationBuilder.DropColumn(
                name: "LastTs",
                schema: "feeshares",
                table: "fee_payments");
        }
    }
}
