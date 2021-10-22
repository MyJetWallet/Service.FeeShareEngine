using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class version_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeeAmountInUsd",
                schema: "feeshares",
                table: "fee_shares",
                newName: "FeeShareAmount");

            migrationBuilder.AddColumn<string>(
                name: "BrokerId",
                schema: "feeshares",
                table: "fee_shares",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConverterWalletId",
                schema: "feeshares",
                table: "fee_shares",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeeShareWalletId",
                schema: "feeshares",
                table: "fee_shares",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrokerId",
                schema: "feeshares",
                table: "fee_shares");

            migrationBuilder.DropColumn(
                name: "ConverterWalletId",
                schema: "feeshares",
                table: "fee_shares");

            migrationBuilder.DropColumn(
                name: "FeeShareWalletId",
                schema: "feeshares",
                table: "fee_shares");

            migrationBuilder.RenameColumn(
                name: "FeeShareAmount",
                schema: "feeshares",
                table: "fee_shares",
                newName: "FeeAmountInUsd");
        }
    }
}
