using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class picsUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconData",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ProfilePictureData",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IconMimeType",
                table: "Restaurants",
                newName: "IconUrl");

            migrationBuilder.RenameColumn(
                name: "ProfilePictureMimeType",
                table: "AspNetUsers",
                newName: "ProfilePictureUrl");

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                table: "Request",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Request_RestaurantId",
                table: "Request",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Restaurants_RestaurantId",
                table: "Request",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Restaurants_RestaurantId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_RestaurantId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "IconUrl",
                table: "Restaurants",
                newName: "IconMimeType");

            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                newName: "ProfilePictureMimeType");

            migrationBuilder.AddColumn<byte[]>(
                name: "IconData",
                table: "Restaurants",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePictureData",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
