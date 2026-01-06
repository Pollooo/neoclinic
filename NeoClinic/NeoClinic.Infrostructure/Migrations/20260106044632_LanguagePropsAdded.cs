using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoClinic.Infrostructure.Migrations
{
    /// <inheritdoc />
    public partial class LanguagePropsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileDescription",
                schema: "clinic",
                table: "MediaFiles");

            migrationBuilder.RenameColumn(
                name: "AltText",
                schema: "clinic",
                table: "MediaFiles",
                newName: "AltTextUz");

            migrationBuilder.RenameColumn(
                name: "Specialty",
                schema: "clinic",
                table: "Doctors",
                newName: "SpecialtyUz");

            migrationBuilder.RenameColumn(
                name: "FullName",
                schema: "clinic",
                table: "Doctors",
                newName: "FullNameUz");

            migrationBuilder.RenameColumn(
                name: "Bio",
                schema: "clinic",
                table: "Doctors",
                newName: "BioUz");

            migrationBuilder.RenameColumn(
                name: "AboutClinic",
                schema: "clinic",
                table: "ContactMessages",
                newName: "AboutClinicUz");

            migrationBuilder.AddColumn<string>(
                name: "AltTextRu",
                schema: "clinic",
                table: "MediaFiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileDescriptionRu",
                schema: "clinic",
                table: "MediaFiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileDescriptionUz",
                schema: "clinic",
                table: "MediaFiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BioRu",
                schema: "clinic",
                table: "Doctors",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullNameRu",
                schema: "clinic",
                table: "Doctors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpecialtyRu",
                schema: "clinic",
                table: "Doctors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboutClinicRu",
                schema: "clinic",
                table: "ContactMessages",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltTextRu",
                schema: "clinic",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "FileDescriptionRu",
                schema: "clinic",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "FileDescriptionUz",
                schema: "clinic",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "BioRu",
                schema: "clinic",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "FullNameRu",
                schema: "clinic",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "SpecialtyRu",
                schema: "clinic",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AboutClinicRu",
                schema: "clinic",
                table: "ContactMessages");

            migrationBuilder.RenameColumn(
                name: "AltTextUz",
                schema: "clinic",
                table: "MediaFiles",
                newName: "AltText");

            migrationBuilder.RenameColumn(
                name: "SpecialtyUz",
                schema: "clinic",
                table: "Doctors",
                newName: "Specialty");

            migrationBuilder.RenameColumn(
                name: "FullNameUz",
                schema: "clinic",
                table: "Doctors",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "BioUz",
                schema: "clinic",
                table: "Doctors",
                newName: "Bio");

            migrationBuilder.RenameColumn(
                name: "AboutClinicUz",
                schema: "clinic",
                table: "ContactMessages",
                newName: "AboutClinic");

            migrationBuilder.AddColumn<string>(
                name: "FileDescription",
                schema: "clinic",
                table: "MediaFiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
