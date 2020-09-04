using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCore.Migrations
{
    public partial class updateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tb_UserRole_Tb_Role_RoleId",
                table: "Tb_UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_Tb_UserRole_Tb_User_UserId",
                table: "Tb_UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tb_UserRole",
                table: "Tb_UserRole");

            migrationBuilder.DropIndex(
                name: "IX_Tb_UserRole_UserId",
                table: "Tb_UserRole");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Tb_UserRole",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "Tb_UserRole",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Tb_UserRole",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tb_UserRole",
                table: "Tb_UserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Tb_UserRole_Tb_Role_RoleId",
                table: "Tb_UserRole",
                column: "RoleId",
                principalTable: "Tb_Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tb_UserRole_Tb_User_UserId",
                table: "Tb_UserRole",
                column: "UserId",
                principalTable: "Tb_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tb_UserRole_Tb_Role_RoleId",
                table: "Tb_UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_Tb_UserRole_Tb_User_UserId",
                table: "Tb_UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tb_UserRole",
                table: "Tb_UserRole");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Tb_UserRole",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "Tb_UserRole",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Tb_UserRole",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tb_UserRole",
                table: "Tb_UserRole",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tb_UserRole_UserId",
                table: "Tb_UserRole",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tb_UserRole_Tb_Role_RoleId",
                table: "Tb_UserRole",
                column: "RoleId",
                principalTable: "Tb_Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tb_UserRole_Tb_User_UserId",
                table: "Tb_UserRole",
                column: "UserId",
                principalTable: "Tb_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
