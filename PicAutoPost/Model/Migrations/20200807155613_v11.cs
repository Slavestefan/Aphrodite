using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OwnerSlaveRelationships",
                columns: table => new
                {
                    IdOwnerSlaveRelationship = table.Column<Guid>(nullable: false),
                    OwnerIdUser = table.Column<Guid>(nullable: false),
                    SlaveIdUser = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerSlaveRelationships", x => x.IdOwnerSlaveRelationship);
                    table.ForeignKey(
                        name: "FK_OwnerSlaveRelationships_Users_OwnerIdUser",
                        column: x => x.OwnerIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnerSlaveRelationships_Users_SlaveIdUser",
                        column: x => x.SlaveIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerSlaveRelationships_OwnerIdUser",
                table: "OwnerSlaveRelationships",
                column: "OwnerIdUser");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerSlaveRelationships_SlaveIdUser_OwnerIdUser",
                table: "OwnerSlaveRelationships",
                columns: new[] { "SlaveIdUser", "OwnerIdUser" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnerSlaveRelationships");
        }
    }
}
