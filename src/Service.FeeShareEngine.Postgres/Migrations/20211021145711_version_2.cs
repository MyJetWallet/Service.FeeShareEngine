using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class version_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "feeshares");

            migrationBuilder.RenameTable(
                name: "share_statistics",
                schema: "balancehistory",
                newName: "share_statistics",
                newSchema: "feeshares");

            migrationBuilder.RenameTable(
                name: "referral_map",
                schema: "balancehistory",
                newName: "referral_map",
                newSchema: "feeshares");

            migrationBuilder.RenameTable(
                name: "fee_shares",
                schema: "balancehistory",
                newName: "fee_shares",
                newSchema: "feeshares");

            migrationBuilder.RenameTable(
                name: "fee_payments",
                schema: "balancehistory",
                newName: "fee_payments",
                newSchema: "feeshares");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "balancehistory");

            migrationBuilder.RenameTable(
                name: "share_statistics",
                schema: "feeshares",
                newName: "share_statistics",
                newSchema: "balancehistory");

            migrationBuilder.RenameTable(
                name: "referral_map",
                schema: "feeshares",
                newName: "referral_map",
                newSchema: "balancehistory");

            migrationBuilder.RenameTable(
                name: "fee_shares",
                schema: "feeshares",
                newName: "fee_shares",
                newSchema: "balancehistory");

            migrationBuilder.RenameTable(
                name: "fee_payments",
                schema: "feeshares",
                newName: "fee_payments",
                newSchema: "balancehistory");
        }
    }
}
