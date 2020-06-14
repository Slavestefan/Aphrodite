using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MultiSet",
                columns: table => new
                {
                    IdMultiSet = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OwnerIdUser = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiSet", x => x.IdMultiSet);
                    table.ForeignKey(
                        name: "FK_MultiSet_Users_OwnerIdUser",
                        column: x => x.OwnerIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MultiSetTaskSet",
                columns: table => new
                {
                    IdMultiSetTaskSet = table.Column<Guid>(nullable: false),
                    IdMultiSet = table.Column<Guid>(nullable: false),
                    IdTaskSet = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiSetTaskSet", x => x.IdMultiSetTaskSet);
                    table.ForeignKey(
                        name: "FK_MultiSetTaskSet_MultiSet_IdMultiSet",
                        column: x => x.IdMultiSet,
                        principalTable: "MultiSet",
                        principalColumn: "IdMultiSet",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MultiSetTaskSet_TaskSets_IdTaskSet",
                        column: x => x.IdTaskSet,
                        principalTable: "TaskSets",
                        principalColumn: "IdTaskSet",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MultiSet_OwnerIdUser",
                table: "MultiSet",
                column: "OwnerIdUser");

            migrationBuilder.CreateIndex(
                name: "IX_MultiSetTaskSet_IdMultiSet",
                table: "MultiSetTaskSet",
                column: "IdMultiSet");

            migrationBuilder.CreateIndex(
                name: "IX_MultiSetTaskSet_IdTaskSet",
                table: "MultiSetTaskSet",
                column: "IdTaskSet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultiSetTaskSet");

            migrationBuilder.DropTable(
                name: "MultiSet");
        }
    }
}
