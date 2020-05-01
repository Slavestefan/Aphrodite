using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    public partial class v9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskSets",
                columns: table => new
                {
                    IdTaskSet = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    OwnerIdUser = table.Column<Guid>(nullable: false),
                    RecipientIdUser = table.Column<Guid>(nullable: true),
                    DoAllowMultiroll = table.Column<bool>(nullable: false),
                    DoesMultirollRepeat = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSets", x => x.IdTaskSet);
                    table.ForeignKey(
                        name: "FK_TaskSets_Users_OwnerIdUser",
                        column: x => x.OwnerIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskSets_Users_RecipientIdUser",
                        column: x => x.RecipientIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    IdTask = table.Column<Guid>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    TaskSetIdTaskSet = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.IdTask);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskSets_TaskSetIdTaskSet",
                        column: x => x.TaskSetIdTaskSet,
                        principalTable: "TaskSets",
                        principalColumn: "IdTaskSet",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskHistories",
                columns: table => new
                {
                    Time = table.Column<DateTime>(nullable: false),
                    TaskIdTask = table.Column<Guid>(nullable: false),
                    PickerIdUser = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHistories", x => x.Time);
                    table.ForeignKey(
                        name: "FK_TaskHistories_Users_PickerIdUser",
                        column: x => x.PickerIdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskHistories_Tasks_TaskIdTask",
                        column: x => x.TaskIdTask,
                        principalTable: "Tasks",
                        principalColumn: "IdTask",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_PickerIdUser",
                table: "TaskHistories",
                column: "PickerIdUser");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_TaskIdTask",
                table: "TaskHistories",
                column: "TaskIdTask");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskSetIdTaskSet",
                table: "Tasks",
                column: "TaskSetIdTaskSet");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSets_OwnerIdUser",
                table: "TaskSets",
                column: "OwnerIdUser");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSets_RecipientIdUser",
                table: "TaskSets",
                column: "RecipientIdUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskHistories");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskSets");
        }
    }
}
