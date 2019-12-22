using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.CreateTable(
                name: "BotConfigurations",
                columns: table => new
                {
                    IdBotConfiguration = table.Column<Guid>(nullable: false),
                    ChannelId = table.Column<decimal>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    ValueString = table.Column<string>(nullable: true),
                    ValueInt = table.Column<int>(nullable: false),
                    ValueBool = table.Column<bool>(nullable: false),
                    ValueUlong = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotConfigurations", x => x.IdBotConfiguration);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotConfigurations_ChannelId_Key",
                table: "BotConfigurations",
                columns: new[] { "ChannelId", "Key" },
                unique: true,
                filter: "[Key] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotConfigurations");

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    IdLog = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.IdLog);
                });
        }
    }
}
