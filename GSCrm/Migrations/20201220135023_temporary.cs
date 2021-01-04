using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GSCrm.Migrations
{
    public partial class temporary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    INN = table.Column<string>(nullable: true),
                    KPP = table.Column<string>(nullable: true),
                    OKPO = table.Column<string>(nullable: true),
                    OGRN = table.Column<string>(nullable: true),
                    Site = table.Column<string>(nullable: true),
                    ParentAccountId = table.Column<Guid>(nullable: false),
                    PrimaryContactId = table.Column<Guid>(nullable: false),
                    LegalAddressId = table.Column<Guid>(nullable: false),
                    PrimaryManagerId = table.Column<Guid>(nullable: false),
                    AccountStatus = table.Column<int>(nullable: false),
                    AccountType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    PrimaryOrganizationId = table.Column<Guid>(nullable: false),
                    DefaultLanguage = table.Column<string>(nullable: false),
                    AvatarPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    DivisionId = table.Column<Guid>(nullable: false),
                    PrimaryPositionId = table.Column<Guid>(nullable: true),
                    EmployeeStatus = table.Column<int>(nullable: false),
                    EmployeeLockReason = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NotificationTarget = table.Column<int>(nullable: false),
                    ActionType = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    HasRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    ManagerId = table.Column<Guid>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    QuoteStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Responsibilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: false),
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
                    EmpUnlock = table.Column<bool>(nullable: false),
                    EmpPossManagement = table.Column<bool>(nullable: false),
                    EmpRespsManagement = table.Column<bool>(nullable: false),
                    EmpContactCreate = table.Column<bool>(nullable: false),
                    EmpContactUpdate = table.Column<bool>(nullable: false),
                    EmpContactDelete = table.Column<bool>(nullable: false),
                    RespCreate = table.Column<bool>(nullable: false),
                    RespUpdate = table.Column<bool>(nullable: false),
                    RespDelete = table.Column<bool>(nullable: false),
                    AccCreate = table.Column<bool>(nullable: false),
                    AccUpdate = table.Column<bool>(nullable: false),
                    AccDelete = table.Column<bool>(nullable: false),
                    AccChangeType = table.Column<bool>(nullable: false),
                    AccUnlock = table.Column<bool>(nullable: false),
                    AccTeamManagement = table.Column<bool>(nullable: false),
                    AccContactCreate = table.Column<bool>(nullable: false),
                    AccContactUpdate = table.Column<bool>(nullable: false),
                    AccContactDelete = table.Column<bool>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "AccountAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressType = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    House = table.Column<string>(nullable: true),
                    AccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountAddresses_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContactType = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    AccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountContacts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BankName = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    CheckingAccount = table.Column<string>(nullable: true),
                    CorrespondentAccount = table.Column<string>(nullable: true),
                    BIC = table.Column<string>(nullable: true),
                    SWIFT = table.Column<string>(nullable: true),
                    AccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountInvoices_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountManagers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    ManagerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountManagers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountManagers_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContactType = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeContacts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    NotificationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ParentDivisionId = table.Column<Guid>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Divisions_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountQuotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    QuoteId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountQuotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountQuotes_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountQuotes_Quotes_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeResponsibilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: false),
                    ResponsibilityId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeResponsibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeResponsibilities_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeResponsibilities_Responsibilities_ResponsibilityId",
                        column: x => x.ResponsibilityId,
                        principalTable: "Responsibilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ParentPositionId = table.Column<Guid>(nullable: true),
                    PrimaryEmployeeId = table.Column<Guid>(nullable: true),
                    DivisionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationsSetting",
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
                    table.PrimaryKey("PK_NotificationsSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationsSetting_UserOrganizations_UserOrganizationId",
                        column: x => x.UserOrganizationId,
                        principalTable: "UserOrganizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PositionId = table.Column<Guid>(nullable: true),
                    EmployeeId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeePositions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeePositions_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountAddresses_AccountId",
                table: "AccountAddresses",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountContacts_AccountId",
                table: "AccountContacts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoices_AccountId",
                table: "AccountInvoices",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountManagers_AccountId",
                table: "AccountManagers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountManagers_ManagerId",
                table: "AccountManagers",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountQuotes_AccountId",
                table: "AccountQuotes",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_OrganizationId",
                table: "Divisions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContacts_EmployeeId",
                table: "EmployeeContacts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePositions_EmployeeId",
                table: "EmployeePositions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePositions_PositionId",
                table: "EmployeePositions",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeResponsibilities_EmployeeId",
                table: "EmployeeResponsibilities",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeResponsibilities_ResponsibilityId",
                table: "EmployeeResponsibilities",
                column: "ResponsibilityId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationsSetting_UserOrganizationId",
                table: "NotificationsSetting",
                column: "UserOrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_DivisionId",
                table: "Positions",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_NotificationId",
                table: "UserNotifications",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId",
                table: "UserNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_OrganizationId",
                table: "UserOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_UserId",
                table: "UserOrganizations",
                column: "UserId");*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropTable(
                name: "AccountAddresses");

            migrationBuilder.DropTable(
                name: "AccountContacts");

            migrationBuilder.DropTable(
                name: "AccountInvoices");

            migrationBuilder.DropTable(
                name: "AccountManagers");

            migrationBuilder.DropTable(
                name: "AccountQuotes");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "EmployeeContacts");

            migrationBuilder.DropTable(
                name: "EmployeePositions");

            migrationBuilder.DropTable(
                name: "EmployeeResponsibilities");

            migrationBuilder.DropTable(
                name: "NotificationsSetting");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Responsibilities");

            migrationBuilder.DropTable(
                name: "UserOrganizations");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Organizations");*/
        }
    }
}
