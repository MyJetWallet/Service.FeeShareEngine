using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class version_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_referral_map_fee_share_groups_FeeShareGroupGroupId",
                schema: "feeshares",
                table: "referral_map");

            migrationBuilder.DropIndex(
                name: "IX_referral_map_FeeShareGroupGroupId",
                schema: "feeshares",
                table: "referral_map");

            migrationBuilder.RenameColumn(
                name: "FeeShareGroupGroupId",
                schema: "feeshares",
                table: "referral_map",
                newName: "FeeShareGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeeShareGroupId",
                schema: "feeshares",
                table: "referral_map",
                newName: "FeeShareGroupGroupId");

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
    }
}
