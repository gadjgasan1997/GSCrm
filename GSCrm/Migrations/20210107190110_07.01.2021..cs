using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _07012021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePositions_Employees_EmployeeId",
                table: "EmployeePositions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePositions_Positions_PositionId",
                table: "EmployeePositions");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePositions_Employees_EmployeeId",
                table: "EmployeePositions",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePositions_Positions_PositionId",
                table: "EmployeePositions",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePositions_Employees_EmployeeId",
                table: "EmployeePositions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePositions_Positions_PositionId",
                table: "EmployeePositions");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePositions_Employees_EmployeeId",
                table: "EmployeePositions",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePositions_Positions_PositionId",
                table: "EmployeePositions",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
