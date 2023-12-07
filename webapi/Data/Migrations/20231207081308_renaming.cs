using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class renaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCodeUrl",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "ResumeUrl",
                table: "Employees",
                newName: "ResumePath");

            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                newName: "ProfilePicturePath");

            migrationBuilder.AddColumn<string>(
                name: "QrCodePath",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCodePath",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "ResumePath",
                table: "Employees",
                newName: "ResumeUrl");

            migrationBuilder.RenameColumn(
                name: "ProfilePicturePath",
                table: "AspNetUsers",
                newName: "ProfilePictureUrl");

            migrationBuilder.AddColumn<string>(
                name: "QrCodeUrl",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
