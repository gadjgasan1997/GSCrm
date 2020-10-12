using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _20092020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Quotes");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Quotes",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "Quotes",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "QuoteStatus",
                table: "Quotes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "QuoteStatus",
                table: "Quotes");

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Quotes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
