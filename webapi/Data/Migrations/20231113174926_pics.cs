using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class pics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
