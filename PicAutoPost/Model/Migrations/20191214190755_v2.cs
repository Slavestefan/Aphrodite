using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.PicAutoPost.Model.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostingIntervalInMinutes",
                table: "Configurations");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChannelId",
                table: "Configurations",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPost",
                table: "Configurations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxPostPerInterval",
                table: "Configurations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxPostingIntervalInMinutes",
                table: "Configurations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinPostPerInterval",
                table: "Configurations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinPostingIntervalInMinutes",
                table: "Configurations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UserId",
                table: "Configurations",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPost",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "MaxPostPerInterval",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "MaxPostingIntervalInMinutes",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "MinPostPerInterval",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "MinPostingIntervalInMinutes",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Configurations");

            migrationBuilder.AlterColumn<long>(
                name: "ChannelId",
                table: "Configurations",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<int>(
                name: "PostingIntervalInMinutes",
                table: "Configurations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
