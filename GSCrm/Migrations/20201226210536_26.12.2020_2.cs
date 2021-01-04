using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _26122020_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Header",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sender",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Notifications",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Header",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Sender",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Notifications");
        }
    }
}
