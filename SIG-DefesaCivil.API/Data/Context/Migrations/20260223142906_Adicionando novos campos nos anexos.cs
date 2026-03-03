using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class Adicionandonovoscamposnosanexos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataHoraCaptura",
                table: "Anexos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LatitudeCaptura",
                table: "Anexos",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LongitudeCaptura",
                table: "Anexos",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataHoraCaptura",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "LatitudeCaptura",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "LongitudeCaptura",
                table: "Anexos");
        }
    }
}
