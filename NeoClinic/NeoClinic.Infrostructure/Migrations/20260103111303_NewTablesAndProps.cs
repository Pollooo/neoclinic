using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoClinic.Infrostructure.Migrations
{
    /// <inheritdoc />
    public partial class NewTablesAndProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlobName",
                schema: "clinic",
                table: "MediaFiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContainerName",
                schema: "clinic",
                table: "MediaFiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                schema: "clinic",
                table: "MediaFiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileDescription",
                schema: "clinic",
                table: "MediaFiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSizeInBytes",
                schema: "clinic",
                table: "MediaFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDoctor",
                schema: "clinic",
                table: "MediaFiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BlobName",
                schema: "clinic",
                table: "Doctors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "clinic",
                table: "Doctors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "TelegramUserId",
                schema: "clinic",
                table: "Admins",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Admins_TelegramUserId",
                schema: "clinic",
                table: "Admins",
                column: "TelegramUserId",
                unique: true);

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

            migrationBuilder.DropIndex(
                name: "IX_Admins_TelegramUserId",
                schema: "clinic",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "BlobName",
                schema: "clinic",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "ContainerName",
                schema: "clinic",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "ContentType",
                schema: "clinic",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "FileDescription",
                schema: "clinic",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "FileSizeInBytes",
                schema: "clinic",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "IsDoctor",
                schema: "clinic",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "BlobName",
                schema: "clinic",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "clinic",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "TelegramUserId",
                schema: "clinic",
                table: "Admins");
        }
    }
}
