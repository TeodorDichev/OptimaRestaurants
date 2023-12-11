using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class managerReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagerReviews_Managers_EmployerId",
                table: "ManagerReviews");

            migrationBuilder.RenameColumn(
                name: "EmployerId",
                table: "ManagerReviews",
                newName: "RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_ManagerReviews_EmployerId",
                table: "ManagerReviews",
                newName: "IX_ManagerReviews_RestaurantId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PunctualityRating",
                table: "ManagerReviews",
                type: "decimal(2,2)",
                precision: 2,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,2)",
                oldPrecision: 2,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "CollegialityRating",
                table: "ManagerReviews",
                type: "decimal(2,2)",
                precision: 2,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,2)",
                oldPrecision: 2,
                oldScale: 2);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerReviews_Restaurants_RestaurantId",
                table: "ManagerReviews",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagerReviews_Restaurants_RestaurantId",
                table: "ManagerReviews");

            migrationBuilder.RenameColumn(
                name: "RestaurantId",
                table: "ManagerReviews",
                newName: "EmployerId");

            migrationBuilder.RenameIndex(
                name: "IX_ManagerReviews_RestaurantId",
                table: "ManagerReviews",
                newName: "IX_ManagerReviews_EmployerId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PunctualityRating",
                table: "ManagerReviews",
                type: "decimal(2,2)",
                precision: 2,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,2)",
                oldPrecision: 2,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CollegialityRating",
                table: "ManagerReviews",
                type: "decimal(2,2)",
                precision: 2,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,2)",
                oldPrecision: 2,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerReviews_Managers_EmployerId",
                table: "ManagerReviews",
                column: "EmployerId",
                principalTable: "Managers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
