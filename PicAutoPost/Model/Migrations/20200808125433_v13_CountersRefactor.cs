using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class v13_CountersRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Counters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Counters");
        }
    }
}
