using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternManagement.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class nouvelle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_weekly_follow_ups_phases_PhaseId",
                table: "weekly_follow_ups");

            migrationBuilder.DropColumn(
                name: "WeekNumber",
                table: "weekly_follow_ups");

            migrationBuilder.AlterColumn<int>(
                name: "PhaseId",
                table: "weekly_follow_ups",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_weekly_follow_ups_phases_PhaseId",
                table: "weekly_follow_ups",
                column: "PhaseId",
                principalTable: "phases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_weekly_follow_ups_phases_PhaseId",
                table: "weekly_follow_ups");

            migrationBuilder.AlterColumn<int>(
                name: "PhaseId",
                table: "weekly_follow_ups",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeekNumber",
                table: "weekly_follow_ups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_weekly_follow_ups_phases_PhaseId",
                table: "weekly_follow_ups",
                column: "PhaseId",
                principalTable: "phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
