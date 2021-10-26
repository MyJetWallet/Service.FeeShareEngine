using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class version_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_share_statistics",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                schema: "feeshares",
                table: "fee_shares",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                schema: "feeshares",
                table: "fee_payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_share_statistics",
                schema: "feeshares",
                table: "share_statistics",
                columns: new[] { "PeriodFrom", "PeriodTo" });

            migrationBuilder.CreateIndex(
                name: "IX_fee_payments_PeriodFrom_PeriodTo",
                schema: "feeshares",
                table: "fee_payments",
                columns: new[] { "PeriodFrom", "PeriodTo" });

            migrationBuilder.CreateIndex(
                name: "IX_fee_payments_Status",
                schema: "feeshares",
                table: "fee_payments",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_share_statistics",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.DropIndex(
                name: "IX_fee_payments_PeriodFrom_PeriodTo",
                schema: "feeshares",
                table: "fee_payments");

            migrationBuilder.DropIndex(
                name: "IX_fee_payments_Status",
                schema: "feeshares",
                table: "fee_payments");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                schema: "feeshares",
                table: "fee_shares");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                schema: "feeshares",
                table: "fee_payments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_share_statistics",
                schema: "feeshares",
                table: "share_statistics",
                column: "PeriodFrom");
        }
    }
}
