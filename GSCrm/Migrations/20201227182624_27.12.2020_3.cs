using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _27122020_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationsSetting");

            migrationBuilder.CreateTable(
                name: "OrgNotificationsSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserOrganizationId = table.Column<Guid>(nullable: false),
                    DivDeleteNot = table.Column<bool>(nullable: false),
                    TDivDeleteNot = table.Column<int>(nullable: false),
                    PosDeleteNot = table.Column<bool>(nullable: false),
                    TPosDeleteNot = table.Column<int>(nullable: false),
                    PosUpdateNot = table.Column<bool>(nullable: false),
                    TPosUpdateNot = table.Column<int>(nullable: false),
                    EmpDeleteNot = table.Column<bool>(nullable: false),
                    TEmpDeleteNot = table.Column<int>(nullable: false),
                    EmpUpdateNot = table.Column<bool>(nullable: false),
                    TEmpUpdateNot = table.Column<int>(nullable: false),
                    AccDeleteNot = table.Column<bool>(nullable: false),
                    TAccDeleteNot = table.Column<int>(nullable: false),
                    AccUpdateNot = table.Column<bool>(nullable: false),
                    TAccUpdateNot = table.Column<int>(nullable: false),
                    AccTeamManagementNot = table.Column<bool>(nullable: false),
                    TAccTeamManagementNot = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgNotificationsSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrgNotificationsSettings_UserOrganizations_UserOrganizationId",
                        column: x => x.UserOrganizationId,
                        principalTable: "UserOrganizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgNotificationsSettings_UserOrganizationId",
                table: "OrgNotificationsSettings",
                column: "UserOrganizationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrgNotificationsSettings");

            migrationBuilder.CreateTable(
                name: "NotificationsSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccDeleteNot = table.Column<bool>(type: "bit", nullable: false),
                    AccTeamManagementNot = table.Column<bool>(type: "bit", nullable: false),
                    AccUpdateNot = table.Column<bool>(type: "bit", nullable: false),
                    DivDeleteNot = table.Column<bool>(type: "bit", nullable: false),
                    EmpDeleteNot = table.Column<bool>(type: "bit", nullable: false),
                    EmpUpdateNot = table.Column<bool>(type: "bit", nullable: false),
                    PosDeleteNot = table.Column<bool>(type: "bit", nullable: false),
                    PosUpdateNot = table.Column<bool>(type: "bit", nullable: false),
                    TAccDeleteNot = table.Column<int>(type: "int", nullable: false),
                    TAccTeamManagementNot = table.Column<int>(type: "int", nullable: false),
                    TAccUpdateNot = table.Column<int>(type: "int", nullable: false),
                    TDivDeleteNot = table.Column<int>(type: "int", nullable: false),
                    TEmpDeleteNot = table.Column<int>(type: "int", nullable: false),
                    TEmpUpdateNot = table.Column<int>(type: "int", nullable: false),
                    TPosDeleteNot = table.Column<int>(type: "int", nullable: false),
                    TPosUpdateNot = table.Column<int>(type: "int", nullable: false),
                    UserOrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationsSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationsSetting_UserOrganizations_UserOrganizationId",
                        column: x => x.UserOrganizationId,
                        principalTable: "UserOrganizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationsSetting_UserOrganizationId",
                table: "NotificationsSetting",
                column: "UserOrganizationId",
                unique: true);
        }
    }
}
