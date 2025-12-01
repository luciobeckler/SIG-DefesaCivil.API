using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class melhoriaquadroAdicionandoquadroseetapasesuasregras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Eventos");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEntradaNaFaseAtual",
                table: "Eventos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EtapaId",
                table: "Eventos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Quadros",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quadros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Etapas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Posicao = table.Column<int>(type: "int", nullable: false),
                    MinTempoNaEtapa = table.Column<TimeSpan>(type: "time", nullable: true),
                    EtapasDestinoId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermissoesParaTransicionarParaEstaEtapa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuadroId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etapas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Etapas_Quadros_QuadroId",
                        column: x => x.QuadroId,
                        principalTable: "Quadros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_EtapaId",
                table: "Eventos",
                column: "EtapaId");

            migrationBuilder.CreateIndex(
                name: "IX_Etapas_QuadroId",
                table: "Etapas",
                column: "QuadroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Etapas_EtapaId",
                table: "Eventos",
                column: "EtapaId",
                principalTable: "Etapas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Etapas_EtapaId",
                table: "Eventos");

            migrationBuilder.DropTable(
                name: "Etapas");

            migrationBuilder.DropTable(
                name: "Quadros");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_EtapaId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "DataEntradaNaFaseAtual",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "EtapaId",
                table: "Eventos");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Eventos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
