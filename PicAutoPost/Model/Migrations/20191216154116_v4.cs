using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInSithMode",
                table: "Configurations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInSithMode",
                table: "Configurations");
        }
    }
}
