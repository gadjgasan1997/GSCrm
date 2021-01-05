using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _050120201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasRead",
                table: "Notifications");

            migrationBuilder.AddColumn<bool>(
                name: "HasRead",
                table: "UserNotifications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasRead",
                table: "UserNotifications");

            migrationBuilder.AddColumn<bool>(
                name: "HasRead",
                table: "Notifications",
                type: "bit",
                nullable: true);
        }
    }
}
