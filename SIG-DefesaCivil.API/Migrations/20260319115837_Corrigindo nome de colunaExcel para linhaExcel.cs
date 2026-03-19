using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class CorrigindonomedecolunaExcelparalinhaExcel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ColunaExcel",
                table: "Ocorrencias",
                newName: "LinhaExcel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LinhaExcel",
                table: "Ocorrencias",
                newName: "ColunaExcel");
        }
    }
}
