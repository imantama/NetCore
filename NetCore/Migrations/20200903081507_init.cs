using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCore.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tb_Role",
                columns: table => new
                {
                    NormalizedName = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tb_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tb_User",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tb_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tb_UserRole",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tb_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tb_UserRole_Tb_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Tb_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tb_UserRole_Tb_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Tb_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tb_UserRole_RoleId",
                table: "Tb_UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tb_UserRole_UserId",
                table: "Tb_UserRole",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tb_UserRole");

            migrationBuilder.DropTable(
                name: "Tb_Role");

            migrationBuilder.DropTable(
                name: "Tb_User");
        }
    }
}
