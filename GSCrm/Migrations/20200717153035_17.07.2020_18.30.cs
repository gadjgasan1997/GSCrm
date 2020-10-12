using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _17072020_1830 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeContacts_Employees_EmpolyeeId",
                table: "EmployeeContacts");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeContacts_EmpolyeeId",
                table: "EmployeeContacts");

            migrationBuilder.DropColumn(
                name: "EmpolyeeId",
                table: "EmployeeContacts");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "EmployeeContacts");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "AccountContacts");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "EmployeeContacts",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumer",
                table: "EmployeeContacts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumer",
                table: "AccountContacts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContacts_EmployeeId",
                table: "EmployeeContacts",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeContacts_Employees_EmployeeId",
                table: "EmployeeContacts",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeContacts_Employees_EmployeeId",
                table: "EmployeeContacts");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeContacts_EmployeeId",
                table: "EmployeeContacts");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "EmployeeContacts");

            migrationBuilder.DropColumn(
                name: "PhoneNumer",
                table: "EmployeeContacts");

            migrationBuilder.DropColumn(
                name: "PhoneNumer",
                table: "AccountContacts");

            migrationBuilder.AddColumn<Guid>(
                name: "EmpolyeeId",
                table: "EmployeeContacts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "EmployeeContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "AccountContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContacts_EmpolyeeId",
                table: "EmployeeContacts",
                column: "EmpolyeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeContacts_Employees_EmpolyeeId",
                table: "EmployeeContacts",
                column: "EmpolyeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
