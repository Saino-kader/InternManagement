using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternManagement.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ajouter MentorId comme nullable pour éviter les violations de FK
            migrationBuilder.AddColumn<int>(
                name: "MentorId",
                table: "feedbacks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_MentorId",
                table: "feedbacks",
                column: "MentorId");

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_mentors_MentorId",
                table: "feedbacks",
                column: "MentorId",
                principalTable: "mentors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_mentors_MentorId",
                table: "feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_feedbacks_MentorId",
                table: "feedbacks");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "feedbacks");
        }
    }
}
