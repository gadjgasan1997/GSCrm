using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _04102020_1613 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserResponsibility");

            migrationBuilder.CreateTable(
                name: "EmployeeResponsibility",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: false),
                    ResponsibilityId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeResponsibility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeResponsibility_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeResponsibility_Responsibilities_ResponsibilityId",
                        column: x => x.ResponsibilityId,
                        principalTable: "Responsibilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeResponsibility_EmployeeId",
                table: "EmployeeResponsibility",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeResponsibility_ResponsibilityId",
                table: "EmployeeResponsibility",
                column: "ResponsibilityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeResponsibility");

            migrationBuilder.CreateTable(
                name: "UserResponsibility",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponsibilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResponsibility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserResponsibility_Responsibilities_ResponsibilityId",
                        column: x => x.ResponsibilityId,
                        principalTable: "Responsibilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserResponsibility_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserResponsibility_ResponsibilityId",
                table: "UserResponsibility",
                column: "ResponsibilityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserResponsibility_UserId",
                table: "UserResponsibility",
                column: "UserId");
        }
    }
}
