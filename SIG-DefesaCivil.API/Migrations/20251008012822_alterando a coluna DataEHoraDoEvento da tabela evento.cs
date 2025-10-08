using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class alterandoacolunaDataEHoraDoEventodatabelaevento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataHora",
                table: "Eventos",
                newName: "DataEHoraDoEvento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataEHoraDoEvento",
                table: "Eventos",
                newName: "DataHora");
        }
    }
}
