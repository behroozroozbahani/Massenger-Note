using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalCore.Persistence.Migrations
{
    public partial class Remove_IsRead_PropertyFromMessengerGroupMessageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "MessengerGroupMessages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "MessengerGroupMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
