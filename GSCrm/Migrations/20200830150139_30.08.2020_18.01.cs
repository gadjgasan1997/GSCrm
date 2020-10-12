using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _30082020_1801 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Site",
                table: "AccountContacts");

            migrationBuilder.AlterColumn<Guid>(
                name: "PrimaryContactId",
                table: "Accounts",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "Accounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AccountContacts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AccountContacts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "AccountContacts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Site",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AccountContacts");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AccountContacts");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "AccountContacts");

            migrationBuilder.AlterColumn<Guid>(
                name: "PrimaryContactId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "AccountContacts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
