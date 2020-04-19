using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class v7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pictures_Hash_UserIdUser",
                table: "Pictures");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Hash",
                table: "Pictures",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(900)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UserAliases",
                columns: table => new
                {
                    IdUserAlias = table.Column<Guid>(nullable: false),
                    UserIdUser = table.Column<Guid>(nullable: true),
                    Alias = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAliases", x => x.IdUserAlias);
                    table.ForeignKey(
                        name: "FK_UserAliases_Users_UserIdUser",
                        column: x => x.UserIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_Hash_UserIdUser",
                table: "Pictures",
                columns: new[] { "Hash", "UserIdUser" },
                unique: true,
                filter: "[UserIdUser] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserAliases_UserIdUser",
                table: "UserAliases",
                column: "UserIdUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAliases");

            migrationBuilder.DropIndex(
                name: "IX_Pictures_Hash_UserIdUser",
                table: "Pictures");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Hash",
                table: "Pictures",
                type: "varbinary(900)",
                nullable: true,
                oldClrType: typeof(byte[]));

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_Hash_UserIdUser",
                table: "Pictures",
                columns: new[] { "Hash", "UserIdUser" },
                unique: true,
                filter: "[Hash] IS NOT NULL AND [UserIdUser] IS NOT NULL");
        }
    }
}
