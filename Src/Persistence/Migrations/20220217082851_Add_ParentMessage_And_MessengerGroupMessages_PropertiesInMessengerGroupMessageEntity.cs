using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalCore.Persistence.Migrations
{
    public partial class Add_ParentMessage_And_MessengerGroupMessages_PropertiesInMessengerGroupMessageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MessengerGroupMessages_ParentId",
                table: "MessengerGroupMessages",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessengerGroupMessages_MessengerGroupMessages_ParentId",
                table: "MessengerGroupMessages",
                column: "ParentId",
                principalTable: "MessengerGroupMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessengerGroupMessages_MessengerGroupMessages_ParentId",
                table: "MessengerGroupMessages");

            migrationBuilder.DropIndex(
                name: "IX_MessengerGroupMessages_ParentId",
                table: "MessengerGroupMessages");
        }
    }
}
