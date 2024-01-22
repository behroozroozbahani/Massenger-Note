using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalCore.Persistence.Migrations
{
    public partial class Add_FileExtentionAndFileMimeAndFileRealNameInMessengerMessageFileEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExtention",
                table: "MessengerMessageFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileMime",
                table: "MessengerMessageFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileRealName",
                table: "MessengerMessageFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtention",
                table: "MessengerMessageFiles");

            migrationBuilder.DropColumn(
                name: "FileMime",
                table: "MessengerMessageFiles");

            migrationBuilder.DropColumn(
                name: "FileRealName",
                table: "MessengerMessageFiles");
        }
    }
}
