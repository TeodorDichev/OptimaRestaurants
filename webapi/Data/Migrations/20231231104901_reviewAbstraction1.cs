using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class reviewAbstraction1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpeedRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    AttitudeRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    CuisineRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    AtmosphereRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReviews_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerReviews_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagerReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PunctualityRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    CollegialityRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagerReviews_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManagerReviews_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReviews_EmployeeId",
                table: "CustomerReviews",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReviews_RestaurantId",
                table: "CustomerReviews",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerReviews_EmployeeId",
                table: "ManagerReviews",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerReviews_RestaurantId",
                table: "ManagerReviews",
                column: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReviews");

            migrationBuilder.DropTable(
                name: "ManagerReviews");
        }
    }
}
