using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class AddLastTsIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_share_statistics_LastTs",
                schema: "feeshares",
                table: "share_statistics",
                column: "LastTs");

            migrationBuilder.CreateIndex(
                name: "IX_referral_map_LastTs",
                schema: "feeshares",
                table: "referral_map",
                column: "LastTs");

            migrationBuilder.CreateIndex(
                name: "IX_fee_shares_LastTs",
                schema: "feeshares",
                table: "fee_shares",
                column: "LastTs");

            migrationBuilder.CreateIndex(
                name: "IX_fee_payments_LastTs",
                schema: "feeshares",
                table: "fee_payments",
                column: "LastTs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_share_statistics_LastTs",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.DropIndex(
                name: "IX_referral_map_LastTs",
                schema: "feeshares",
                table: "referral_map");

            migrationBuilder.DropIndex(
                name: "IX_fee_shares_LastTs",
                schema: "feeshares",
                table: "fee_shares");

            migrationBuilder.DropIndex(
                name: "IX_fee_payments_LastTs",
                schema: "feeshares",
                table: "fee_payments");
        }
    }
}
