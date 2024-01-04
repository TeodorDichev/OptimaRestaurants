using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class reviewAbstraction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReviews");

            migrationBuilder.DropTable(
                name: "ManagerReviews");

            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropColumn(
                name: "MinRatingForBonuses",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "OverWorkingAmountPerHour",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "OverWorkingHours",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "RatingBonusesAmount",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "StandardMonthlyPayment",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "UsePercentageGrowth",
                table: "Restaurants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinRatingForBonuses",
                table: "Restaurants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OverWorkingAmountPerHour",
                table: "Restaurants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OverWorkingHours",
                table: "Restaurants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RatingBonusesAmount",
                table: "Restaurants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StandardMonthlyPayment",
                table: "Restaurants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "UsePercentageGrowth",
                table: "Restaurants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CustomerReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtmosphereRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    AttitudeRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CuisineRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SpeedRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true)
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
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollegialityRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunctualityRating = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FixedSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OverWorkingBonuses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RatingBonuses = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
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

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_EmployeeId",
                table: "Transfers",
                column: "EmployeeId");
        }
    }
}
