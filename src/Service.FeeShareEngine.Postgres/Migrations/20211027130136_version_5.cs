using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class version_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_share_statistics",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_fee_payments",
                schema: "feeshares",
                table: "fee_payments");

            migrationBuilder.AddColumn<string>(
                name: "AssetId",
                schema: "feeshares",
                table: "share_statistics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SettlementOperationId",
                schema: "feeshares",
                table: "share_statistics",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "feeshares",
                table: "share_statistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FeeShareGroupGroupId",
                schema: "feeshares",
                table: "referral_map",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetId",
                schema: "feeshares",
                table: "fee_payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_share_statistics",
                schema: "feeshares",
                table: "share_statistics",
                columns: new[] { "PeriodFrom", "PeriodTo", "AssetId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_fee_payments",
                schema: "feeshares",
                table: "fee_payments",
                columns: new[] { "ReferrerClientId", "PeriodFrom", "PeriodTo", "AssetId" });

            migrationBuilder.CreateTable(
                name: "fee_share_groups",
                schema: "feeshares",
                columns: table => new
                {
                    GroupId = table.Column<string>(type: "text", nullable: false),
                    AssetId = table.Column<string>(type: "text", nullable: true),
                    FeePercent = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fee_share_groups", x => x.GroupId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_referral_map_FeeShareGroupGroupId",
                schema: "feeshares",
                table: "referral_map",
                column: "FeeShareGroupGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_referral_map_fee_share_groups_FeeShareGroupGroupId",
                schema: "feeshares",
                table: "referral_map",
                column: "FeeShareGroupGroupId",
                principalSchema: "feeshares",
                principalTable: "fee_share_groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_referral_map_fee_share_groups_FeeShareGroupGroupId",
                schema: "feeshares",
                table: "referral_map");

            migrationBuilder.DropTable(
                name: "fee_share_groups",
                schema: "feeshares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_share_statistics",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.DropIndex(
                name: "IX_referral_map_FeeShareGroupGroupId",
                schema: "feeshares",
                table: "referral_map");

            migrationBuilder.DropPrimaryKey(
                name: "PK_fee_payments",
                schema: "feeshares",
                table: "fee_payments");

            migrationBuilder.DropColumn(
                name: "AssetId",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.DropColumn(
                name: "SettlementOperationId",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "feeshares",
                table: "share_statistics");

            migrationBuilder.DropColumn(
                name: "FeeShareGroupGroupId",
                schema: "feeshares",
                table: "referral_map");

            migrationBuilder.DropColumn(
                name: "AssetId",
                schema: "feeshares",
                table: "fee_payments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_share_statistics",
                schema: "feeshares",
                table: "share_statistics",
                columns: new[] { "PeriodFrom", "PeriodTo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_fee_payments",
                schema: "feeshares",
                table: "fee_payments",
                columns: new[] { "ReferrerClientId", "PeriodFrom" });
        }
    }
}
