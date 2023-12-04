using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class requestDecember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_AspNetUsers_SenderId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_Restaurants_RestaurantId",
                table: "Request");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Request",
                table: "Request");

            migrationBuilder.RenameTable(
                name: "Request",
                newName: "Requests");

            migrationBuilder.RenameIndex(
                name: "IX_Request_SenderId",
                table: "Requests",
                newName: "IX_Requests_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RestaurantId",
                table: "Requests",
                newName: "IX_Requests_RestaurantId");

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Requests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "Requests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                table: "Requests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ReceiverId",
                table: "Requests",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_ReceiverId",
                table: "Requests",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_SenderId",
                table: "Requests",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Restaurants_RestaurantId",
                table: "Requests",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_ReceiverId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_SenderId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Restaurants_RestaurantId",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Requests",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ReceiverId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Requests");

            migrationBuilder.RenameTable(
                name: "Requests",
                newName: "Request");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_SenderId",
                table: "Request",
                newName: "IX_Request_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_RestaurantId",
                table: "Request",
                newName: "IX_Request_RestaurantId");

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Request",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Request",
                table: "Request",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AspNetUsers_SenderId",
                table: "Request",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Restaurants_RestaurantId",
                table: "Request",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
