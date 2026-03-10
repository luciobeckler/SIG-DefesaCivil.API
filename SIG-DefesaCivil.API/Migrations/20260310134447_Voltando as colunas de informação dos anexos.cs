using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class Voltandoascolunasdeinformaçãodosanexos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NomeOriginal",
                table: "Anexos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "TamanhoBytes",
                table: "Anexos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "TipoConteudo",
                table: "Anexos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomeOriginal",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "TamanhoBytes",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "TipoConteudo",
                table: "Anexos");
        }
    }
}
