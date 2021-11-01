using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.FeeShareEngine.Postgres.Migrations
{
    public partial class version_11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "feeshares",
                table: "fee_share_groups",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "feeshares",
                table: "fee_share_groups");
        }
    }
}
