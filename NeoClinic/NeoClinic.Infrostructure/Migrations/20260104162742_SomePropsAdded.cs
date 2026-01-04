using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoClinic.Infrostructure.Migrations
{
    /// <inheritdoc />
    public partial class SomePropsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "clinic",
                table: "Services",
                newName: "NameUz");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "clinic",
                table: "Services",
                newName: "DescriptionUz");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeveloper",
                schema: "clinic",
                table: "TelegramUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsManager",
                schema: "clinic",
                table: "TelegramUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                schema: "clinic",
                table: "TelegramUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionRu",
                schema: "clinic",
                table: "Services",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                schema: "clinic",
                table: "Services",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeveloper",
                schema: "clinic",
                table: "TelegramUsers");

            migrationBuilder.DropColumn(
                name: "IsManager",
                schema: "clinic",
                table: "TelegramUsers");

            migrationBuilder.DropColumn(
                name: "Language",
                schema: "clinic",
                table: "TelegramUsers");

            migrationBuilder.DropColumn(
                name: "DescriptionRu",
                schema: "clinic",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "NameRu",
                schema: "clinic",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "NameUz",
                schema: "clinic",
                table: "Services",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionUz",
                schema: "clinic",
                table: "Services",
                newName: "Description");
        }
    }
}
