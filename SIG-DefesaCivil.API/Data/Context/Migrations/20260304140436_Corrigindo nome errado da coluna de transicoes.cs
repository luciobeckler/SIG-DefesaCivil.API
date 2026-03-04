using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class Corrigindonomeerradodacolunadetransicoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trasicoes_Etapas_EtapaAnteriorlId",
                table: "Trasicoes");

            migrationBuilder.RenameColumn(
                name: "EtapaAnteriorlId",
                table: "Trasicoes",
                newName: "EtapaAnteriorId");

            migrationBuilder.RenameIndex(
                name: "IX_Trasicoes_EtapaAnteriorlId",
                table: "Trasicoes",
                newName: "IX_Trasicoes_EtapaAnteriorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trasicoes_Etapas_EtapaAnteriorId",
                table: "Trasicoes",
                column: "EtapaAnteriorId",
                principalTable: "Etapas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trasicoes_Etapas_EtapaAnteriorId",
                table: "Trasicoes");

            migrationBuilder.RenameColumn(
                name: "EtapaAnteriorId",
                table: "Trasicoes",
                newName: "EtapaAnteriorlId");

            migrationBuilder.RenameIndex(
                name: "IX_Trasicoes_EtapaAnteriorId",
                table: "Trasicoes",
                newName: "IX_Trasicoes_EtapaAnteriorlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trasicoes_Etapas_EtapaAnteriorlId",
                table: "Trasicoes",
                column: "EtapaAnteriorlId",
                principalTable: "Etapas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
