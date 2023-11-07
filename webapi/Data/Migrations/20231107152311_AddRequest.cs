using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class AddRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmedOn",
                table: "EmployeesRestaurants");

            migrationBuilder.RenameColumn(
                name: "SentOn",
                table: "EmployeesRestaurants",
                newName: "StartedOn");

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SentOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Request_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_SenderId",
                table: "Request",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.RenameColumn(
                name: "StartedOn",
                table: "EmployeesRestaurants",
                newName: "SentOn");

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedOn",
                table: "EmployeesRestaurants",
                type: "datetime2",
                nullable: true);
        }
    }
}
