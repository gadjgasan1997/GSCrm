using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _26122020_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Sender",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "ActionType",
                table: "Notifications",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ActionType",
                table: "Notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sender",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
