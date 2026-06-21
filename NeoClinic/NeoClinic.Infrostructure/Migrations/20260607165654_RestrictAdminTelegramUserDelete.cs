using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoClinic.Infrostructure.Migrations
{
    /// <inheritdoc />
    public partial class RestrictAdminTelegramUserDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_TelegramUsers_TelegramUserId",
                schema: "clinic",
                table: "Admins");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_TelegramUsers_TelegramUserId",
                schema: "clinic",
                table: "Admins",
                column: "TelegramUserId",
                principalSchema: "clinic",
                principalTable: "TelegramUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_TelegramUsers_TelegramUserId",
                schema: "clinic",
                table: "Admins");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_TelegramUsers_TelegramUserId",
                schema: "clinic",
                table: "Admins",
                column: "TelegramUserId",
                principalSchema: "clinic",
                principalTable: "TelegramUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
