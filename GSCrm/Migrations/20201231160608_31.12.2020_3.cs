using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _31122020_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Accepted",
                table: "UserOrganizations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accepted",
                table: "UserOrganizations");
        }
    }
}
