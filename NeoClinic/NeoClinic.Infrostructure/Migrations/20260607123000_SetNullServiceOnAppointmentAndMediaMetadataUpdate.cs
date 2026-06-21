using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoClinic.Infrostructure.Migrations
{
    /// <inheritdoc />
    public partial class SetNullServiceOnAppointmentAndMediaMetadataUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                schema: "clinic",
                table: "Appointments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceId",
                schema: "clinic",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                schema: "clinic",
                table: "Appointments",
                column: "ServiceId",
                principalSchema: "clinic",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                schema: "clinic",
                table: "Appointments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceId",
                schema: "clinic",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                schema: "clinic",
                table: "Appointments",
                column: "ServiceId",
                principalSchema: "clinic",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
