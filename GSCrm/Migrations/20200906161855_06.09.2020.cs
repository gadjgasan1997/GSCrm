using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _06092020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumer",
                table: "EmployeeContacts");

            migrationBuilder.DropColumn(
                name: "PhoneNumer",
                table: "AccountContacts");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "EmployeeContacts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "AccountContacts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "EmployeeContacts");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AccountContacts");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumer",
                table: "EmployeeContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumer",
                table: "AccountContacts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
