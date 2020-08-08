using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class v14_CounterHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CounterHistory",
                columns: table => new
                {
                    IdCounterHistory = table.Column<Guid>(nullable: false),
                    CounterIdCounter = table.Column<Guid>(nullable: false),
                    AmountChanged = table.Column<int>(nullable: false),
                    ChangeType = table.Column<int>(nullable: false),
                    ByUserIdUser = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterHistory", x => x.IdCounterHistory);
                    table.ForeignKey(
                        name: "FK_CounterHistory_Users_ByUserIdUser",
                        column: x => x.ByUserIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CounterHistory_Counters_CounterIdCounter",
                        column: x => x.CounterIdCounter,
                        principalTable: "Counters",
                        principalColumn: "IdCounter",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CounterHistory_ByUserIdUser",
                table: "CounterHistory",
                column: "ByUserIdUser");

            migrationBuilder.CreateIndex(
                name: "IX_CounterHistory_CounterIdCounter",
                table: "CounterHistory",
                column: "CounterIdCounter");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CounterHistory");
        }
    }
}
