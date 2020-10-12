using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class _04102020_1605 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Responsibilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OrgDelete = table.Column<bool>(nullable: false),
                    DivCreate = table.Column<bool>(nullable: false),
                    DivDelete = table.Column<bool>(nullable: false),
                    PosCreate = table.Column<bool>(nullable: false),
                    PosDelete = table.Column<bool>(nullable: false),
                    PosUpdate = table.Column<bool>(nullable: false),
                    PosChangeDiv = table.Column<bool>(nullable: false),
                    EmpCreate = table.Column<bool>(nullable: false),
                    EmpDelete = table.Column<bool>(nullable: false),
                    EmpUpdate = table.Column<bool>(nullable: false),
                    EmpChangeDiv = table.Column<bool>(nullable: false),
                    EmpPosDelete = table.Column<bool>(nullable: false),
                    EmpPossManagement = table.Column<bool>(nullable: false),
                    EmpContactCreate = table.Column<bool>(nullable: false),
                    EmpContactDelete = table.Column<bool>(nullable: false),
                    AccCreate = table.Column<bool>(nullable: false),
                    AccUpdate = table.Column<bool>(nullable: false),
                    AccDelete = table.Column<bool>(nullable: false),
                    AccChangeType = table.Column<bool>(nullable: false),
                    AccTeamManagement = table.Column<bool>(nullable: false),
                    AccContactCreate = table.Column<bool>(nullable: false),
                    AccContactUpdate = table.Column<bool>(nullable: false),
                    AccContactDelete = table.Column<bool>(nullable: false),
                    AccContactChangePrimary = table.Column<bool>(nullable: false),
                    AccAddressCreate = table.Column<bool>(nullable: false),
                    AccAddressUpdate = table.Column<bool>(nullable: false),
                    AccAddressDelete = table.Column<bool>(nullable: false),
                    AccInvoiceCreate = table.Column<bool>(nullable: false),
                    AccInvoiceUpdate = table.Column<bool>(nullable: false),
                    AccInvoiceDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsibilities", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Responsibilities");
        }
    }
}
