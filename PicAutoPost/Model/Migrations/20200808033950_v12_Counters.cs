using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class v12_Counters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Counters",
                columns: table => new
                {
                    IdCounter = table.Column<Guid>(nullable: false),
                    TotalAmount = table.Column<int>(nullable: false),
                    CompletedAmount = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    UserIdUser = table.Column<Guid>(nullable: true),
                    IsHidden = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counters", x => x.IdCounter);
                    table.ForeignKey(
                        name: "FK_Counters_Users_UserIdUser",
                        column: x => x.UserIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Counters_UserIdUser",
                table: "Counters",
                column: "UserIdUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Counters");
        }
    }
}
