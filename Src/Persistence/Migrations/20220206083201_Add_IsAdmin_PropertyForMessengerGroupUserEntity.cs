using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalCore.Persistence.Migrations
{
    public partial class Add_IsAdmin_PropertyForMessengerGroupUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "MessengerGroups");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "MessengerGroupUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "MessengerGroupUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "MessengerGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
