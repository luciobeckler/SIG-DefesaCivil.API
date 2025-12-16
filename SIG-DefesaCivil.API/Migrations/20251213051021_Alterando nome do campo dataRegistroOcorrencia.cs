using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class AlterandonomedocampodataRegistroOcorrencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataEHoraDaSolicitacao",
                table: "Ocorrencias",
                newName: "DataEHoraDoOcorrido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataEHoraDoOcorrido",
                table: "Ocorrencias",
                newName: "DataEHoraDaSolicitacao");
        }
    }
}
