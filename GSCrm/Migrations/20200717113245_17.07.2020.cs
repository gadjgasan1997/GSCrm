using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _17072020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeContacts_Empolyees_EmpolyeeId",
                table: "EmployeeContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpolyeePositions_Empolyees_EmployeeId",
                table: "EmpolyeePositions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Empolyees",
                table: "Empolyees");

            migrationBuilder.DropColumn(
                name: "PrimaryEmployeeId",
                table: "Positions");

            migrationBuilder.RenameTable(
                name: "Empolyees",
                newName: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                table: "EmployeeContacts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                table: "AccountContacts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeContacts_Employees_EmpolyeeId",
                table: "EmployeeContacts",
                column: "EmpolyeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpolyeePositions_Employees_EmployeeId",
                table: "EmpolyeePositions",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeContacts_Employees_EmpolyeeId",
                table: "EmployeeContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpolyeePositions_Employees_EmployeeId",
                table: "EmpolyeePositions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "EmployeeContacts");

            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "AccountContacts");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Empolyees");

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryEmployeeId",
                table: "Positions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Empolyees",
                table: "Empolyees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeContacts_Empolyees_EmpolyeeId",
                table: "EmployeeContacts",
                column: "EmpolyeeId",
                principalTable: "Empolyees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpolyeePositions_Empolyees_EmployeeId",
                table: "EmpolyeePositions",
                column: "EmployeeId",
                principalTable: "Empolyees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
