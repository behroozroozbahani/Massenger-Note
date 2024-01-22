using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalCore.Persistence.Migrations
{
    public partial class ChangeName_FileThumbNailAndFileExtensionPropertyInMessengerMessageFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileThumNail",
                table: "MessengerMessageFiles",
                newName: "FileThumbNail");

            migrationBuilder.RenameColumn(
                name: "FileExtention",
                table: "MessengerMessageFiles",
                newName: "FileExtension");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileThumbNail",
                table: "MessengerMessageFiles",
                newName: "FileThumNail");

            migrationBuilder.RenameColumn(
                name: "FileExtension",
                table: "MessengerMessageFiles",
                newName: "FileExtention");
        }
    }
}
