using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _16072020_2056 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpolyeePosition_Empolyees_EmployeeId",
                table: "EmpolyeePosition");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpolyeePosition_Positions_PositionId",
                table: "EmpolyeePosition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpolyeePosition",
                table: "EmpolyeePosition");

            migrationBuilder.RenameTable(
                name: "EmpolyeePosition",
                newName: "EmpolyeePositions");

            migrationBuilder.RenameIndex(
                name: "IX_EmpolyeePosition_PositionId",
                table: "EmpolyeePositions",
                newName: "IX_EmpolyeePositions_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_EmpolyeePosition_EmployeeId",
                table: "EmpolyeePositions",
                newName: "IX_EmpolyeePositions_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpolyeePositions",
                table: "EmpolyeePositions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserOrganizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrganizations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_OrganizationId",
                table: "UserOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_UserId",
                table: "UserOrganizations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpolyeePositions_Empolyees_EmployeeId",
                table: "EmpolyeePositions",
                column: "EmployeeId",
                principalTable: "Empolyees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpolyeePositions_Positions_PositionId",
                table: "EmpolyeePositions",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpolyeePositions_Empolyees_EmployeeId",
                table: "EmpolyeePositions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpolyeePositions_Positions_PositionId",
                table: "EmpolyeePositions");

            migrationBuilder.DropTable(
                name: "UserOrganizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpolyeePositions",
                table: "EmpolyeePositions");

            migrationBuilder.RenameTable(
                name: "EmpolyeePositions",
                newName: "EmpolyeePosition");

            migrationBuilder.RenameIndex(
                name: "IX_EmpolyeePositions_PositionId",
                table: "EmpolyeePosition",
                newName: "IX_EmpolyeePosition_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_EmpolyeePositions_EmployeeId",
                table: "EmpolyeePosition",
                newName: "IX_EmpolyeePosition_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpolyeePosition",
                table: "EmpolyeePosition",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpolyeePosition_Empolyees_EmployeeId",
                table: "EmpolyeePosition",
                column: "EmployeeId",
                principalTable: "Empolyees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpolyeePosition_Positions_PositionId",
                table: "EmpolyeePosition",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
