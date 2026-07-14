using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternManagement.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class RemovePhaseNumberFromImportedFollowUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhaseNumber",
                table: "imported_followups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhaseNumber",
                table: "imported_followups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
