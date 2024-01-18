using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Schedules");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedOn",
                table: "Schedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedOn",
                table: "Schedules");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Schedules",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }
    }
}
