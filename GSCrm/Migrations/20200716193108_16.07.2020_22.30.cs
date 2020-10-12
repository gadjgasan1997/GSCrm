using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _16072020_2230 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOrganizations_AspNetUsers_UserId",
                table: "UserOrganizations");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrganizations_AspNetUsers_UserId",
                table: "UserOrganizations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOrganizations_AspNetUsers_UserId",
                table: "UserOrganizations");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrganizations_AspNetUsers_UserId",
                table: "UserOrganizations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
