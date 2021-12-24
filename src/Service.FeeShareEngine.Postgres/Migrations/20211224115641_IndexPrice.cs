using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class IndexPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FeeAssetIndexPrice",
                schema: "feeshares",
                table: "fee_shares",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TargetAssetIndexPrice",
                schema: "feeshares",
                table: "fee_shares",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AssetIndexPrice",
                schema: "feeshares",
                table: "fee_payments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeeAssetIndexPrice",
                schema: "feeshares",
                table: "fee_shares");

            migrationBuilder.DropColumn(
                name: "TargetAssetIndexPrice",
                schema: "feeshares",
                table: "fee_shares");

            migrationBuilder.DropColumn(
                name: "AssetIndexPrice",
                schema: "feeshares",
                table: "fee_payments");
        }
    }
}
