using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternManagement.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToWeek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "weeks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "weeks");
        }
    }
}
