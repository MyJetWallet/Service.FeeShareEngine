using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class fixFeshareIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_fee_share_groups_LastTs",
                schema: "feeshares",
                table: "fee_share_groups",
                column: "LastTs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_fee_share_groups_LastTs",
                schema: "feeshares",
                table: "fee_share_groups");
        }
    }
}
