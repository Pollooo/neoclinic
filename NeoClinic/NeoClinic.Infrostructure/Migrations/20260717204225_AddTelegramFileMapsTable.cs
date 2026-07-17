using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoClinic.Infrostructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTelegramFileMapsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelegramFileMaps",
                schema: "clinic",
                columns: table => new
                {
                    BlobName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramFileMaps", x => x.BlobName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramFileMaps",
                schema: "clinic");
        }
    }
}
