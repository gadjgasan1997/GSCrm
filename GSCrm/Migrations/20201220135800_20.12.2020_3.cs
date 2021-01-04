using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _20122020_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TDivDeleteNots",
                table: "NotificationsSetting");

            migrationBuilder.AddColumn<int>(
                name: "TDivDeleteNot",
                table: "NotificationsSetting",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TDivDeleteNot",
                table: "NotificationsSetting");

            migrationBuilder.AddColumn<int>(
                name: "TDivDeleteNots",
                table: "NotificationsSetting",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
