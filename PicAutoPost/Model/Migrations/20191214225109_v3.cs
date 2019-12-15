using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.PicAutoPost.Model.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    IdLog = table.Column<Guid>(nullable: false),
                    ChannelName = table.Column<string>(nullable: true),
                    ChannelId = table.Column<decimal>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    AuthorId = table.Column<decimal>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.IdLog);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
