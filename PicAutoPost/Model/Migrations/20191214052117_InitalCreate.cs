using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class InitalCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    IdConfiguration = table.Column<Guid>(nullable: false),
                    ChannelId = table.Column<long>(nullable: false),
                    PostingIntervalInMinutes = table.Column<int>(nullable: false),
                    IsRunning = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.IdConfiguration);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdUser = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    DiscordId = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "PicturePool",
                columns: table => new
                {
                    IdPicturePool = table.Column<Guid>(nullable: false),
                    OwnerIdUser = table.Column<Guid>(nullable: true),
                    PostConfigurationIdConfiguration = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PicturePool", x => x.IdPicturePool);
                    table.ForeignKey(
                        name: "FK_PicturePool_Users_OwnerIdUser",
                        column: x => x.OwnerIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PicturePool_Configurations_PostConfigurationIdConfiguration",
                        column: x => x.PostConfigurationIdConfiguration,
                        principalTable: "Configurations",
                        principalColumn: "IdConfiguration",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    IdPicture = table.Column<Guid>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Raw = table.Column<byte[]>(nullable: true),
                    UserIdUser = table.Column<Guid>(nullable: true),
                    PicturePoolIdPicturePool = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.IdPicture);
                    table.ForeignKey(
                        name: "FK_Pictures_PicturePool_PicturePoolIdPicturePool",
                        column: x => x.PicturePoolIdPicturePool,
                        principalTable: "PicturePool",
                        principalColumn: "IdPicturePool",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pictures_Users_UserIdUser",
                        column: x => x.UserIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PicturePool_OwnerIdUser",
                table: "PicturePool",
                column: "OwnerIdUser");

            migrationBuilder.CreateIndex(
                name: "IX_PicturePool_PostConfigurationIdConfiguration",
                table: "PicturePool",
                column: "PostConfigurationIdConfiguration");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_PicturePoolIdPicturePool",
                table: "Pictures",
                column: "PicturePoolIdPicturePool");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_UserIdUser",
                table: "Pictures",
                column: "UserIdUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pictures");

            migrationBuilder.DropTable(
                name: "PicturePool");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Configurations");
        }
    }
}
