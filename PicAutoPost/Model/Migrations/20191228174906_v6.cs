using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class v6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Hash",
                table: "Pictures",
                nullable: true);

            migrationBuilder.Sql("UPDATE Pictures SET [Hash] = HASHBYTES('SHA2_256',[Raw])");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Hash",
                table: "Pictures",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(900)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_Hash_UserIdUser",
                table: "Pictures",
                columns: new[] { "Hash", "UserIdUser" },
                unique: true,
                filter: "[Hash] IS NOT NULL AND [UserIdUser] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pictures_Hash_UserIdUser",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Pictures");
        }
    }
}
