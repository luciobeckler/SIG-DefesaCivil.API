using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class Refatorandocamposdeocorrencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Anexos_EntidadeId_TipoEntidade",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "Localizacao_Numero",
                table: "Ocorrencias");

            migrationBuilder.RenameColumn(
                name: "TipoDeRisco",
                table: "Ocorrencias",
                newName: "Campos_TipoDeRisco");

            migrationBuilder.RenameColumn(
                name: "TipificacaoDaOcorrencia",
                table: "Ocorrencias",
                newName: "Campos_TipificacaoDaOcorrencia");

            migrationBuilder.RenameColumn(
                name: "SolicitanteRG",
                table: "Ocorrencias",
                newName: "Campos_SolicitanteRG");

            migrationBuilder.RenameColumn(
                name: "SolicitanteNome",
                table: "Ocorrencias",
                newName: "Campos_SolicitanteNome");

            migrationBuilder.RenameColumn(
                name: "SolicitanteCPF",
                table: "Ocorrencias",
                newName: "Campos_SolicitanteCPF");

            migrationBuilder.RenameColumn(
                name: "RegimeDeOcupacaoDoImovel",
                table: "Ocorrencias",
                newName: "Campos_RegimeDeOcupacaoDoImovel");

            migrationBuilder.RenameColumn(
                name: "PossuiUnidadeFamiliar",
                table: "Ocorrencias",
                newName: "Campos_PossuiUnidadeFamiliar");

            migrationBuilder.RenameColumn(
                name: "PossuiIPTU",
                table: "Ocorrencias",
                newName: "Campos_PossuiIPTU");

            migrationBuilder.RenameColumn(
                name: "NumeroDePavimentos",
                table: "Ocorrencias",
                newName: "Campos_NumeroDePavimentos");

            migrationBuilder.RenameColumn(
                name: "NumeroDeMoradias",
                table: "Ocorrencias",
                newName: "Campos_NumeroDeMoradias");

            migrationBuilder.RenameColumn(
                name: "NumeroDeIdosos",
                table: "Ocorrencias",
                newName: "Campos_NumeroDeIdosos");

            migrationBuilder.RenameColumn(
                name: "NumeroDeDeficientes",
                table: "Ocorrencias",
                newName: "Campos_NumeroDeDeficientes");

            migrationBuilder.RenameColumn(
                name: "NumeroDeCriancas",
                table: "Ocorrencias",
                newName: "Campos_NumeroDeCriancas");

            migrationBuilder.RenameColumn(
                name: "NumeroDeComodos",
                table: "Ocorrencias",
                newName: "Campos_NumeroDeComodos");

            migrationBuilder.RenameColumn(
                name: "NumeroDeAdultos",
                table: "Ocorrencias",
                newName: "Campos_NumeroDeAdultos");

            migrationBuilder.RenameColumn(
                name: "Numero",
                table: "Ocorrencias",
                newName: "Campos_Localizacao_Numero");

            migrationBuilder.RenameColumn(
                name: "Motivacao",
                table: "Ocorrencias",
                newName: "Campos_Motivacao");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Rua",
                table: "Ocorrencias",
                newName: "Campos_Localizacao_Rua");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Longitude",
                table: "Ocorrencias",
                newName: "Campos_Localizacao_Longitude");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Latitude",
                table: "Ocorrencias",
                newName: "Campos_Localizacao_Latitude");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Complemento",
                table: "Ocorrencias",
                newName: "Campos_Localizacao_Complemento");

            migrationBuilder.RenameColumn(
                name: "Localizacao_CEP",
                table: "Ocorrencias",
                newName: "Campos_Localizacao_CEP");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Bairro",
                table: "Ocorrencias",
                newName: "Campos_Localizacao_Bairro");

            migrationBuilder.RenameColumn(
                name: "GrauDeRisco",
                table: "Ocorrencias",
                newName: "Campos_GrauDeRisco");

            migrationBuilder.RenameColumn(
                name: "Estrutura",
                table: "Ocorrencias",
                newName: "Campos_Estrutura");

            migrationBuilder.RenameColumn(
                name: "Edificacao",
                table: "Ocorrencias",
                newName: "Campos_Edificacao");

            migrationBuilder.RenameColumn(
                name: "DataEHoraTerminoAtendimento",
                table: "Ocorrencias",
                newName: "Campos_DataEHoraTerminoAtendimento");

            migrationBuilder.RenameColumn(
                name: "DataEHoraInicioAtendimento",
                table: "Ocorrencias",
                newName: "Campos_DataEHoraInicioAtendimento");

            migrationBuilder.RenameColumn(
                name: "DataEHoraDoOcorrido",
                table: "Ocorrencias",
                newName: "Campos_DataEHoraDoOcorrido");

            migrationBuilder.RenameColumn(
                name: "CaracterizacaoDoLocal",
                table: "Ocorrencias",
                newName: "Campos_CaracterizacaoDoLocal");

            migrationBuilder.RenameColumn(
                name: "AreasAfetadas",
                table: "Ocorrencias",
                newName: "Campos_AreasAfetadas");

            migrationBuilder.RenameColumn(
                name: "AnalisePreliminar",
                table: "Ocorrencias",
                newName: "Campos_AnalisePreliminar");

            migrationBuilder.RenameColumn(
                name: "isVisible",
                table: "Ocorrencias",
                newName: "isVisivel");

            migrationBuilder.AlterColumn<string>(
                name: "Campos_Localizacao_Numero",
                table: "Ocorrencias",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Protocolo",
                table: "Ocorrencias",
                type: "text",
                nullable: false,
                defaultValue: "");

            // No método Up da Migration, antes de qualquer alteração na coluna:
            migrationBuilder.Sql("UPDATE \"Anexos\" SET \"TipoEntidade\" = '1' WHERE \"TipoEntidade\" = 'Ocorrencia'");
            migrationBuilder.Sql("UPDATE \"Anexos\" SET \"TipoEntidade\" = '2' WHERE \"TipoEntidade\" = 'Usuario'");

            // Agora sim, converte o tipo da coluna
            migrationBuilder.Sql("ALTER TABLE \"Anexos\" ALTER COLUMN \"TipoEntidade\" TYPE integer USING \"TipoEntidade\"::integer;");
            migrationBuilder.CreateTable(
                name: "Trasicoes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DataEHorario = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OcorrenciaId = table.Column<string>(type: "text", nullable: false),
                    ResponsavelId = table.Column<string>(type: "text", nullable: false),
                    EtapaAtualId = table.Column<string>(type: "text", nullable: false),
                    EtapaAnteriorlId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trasicoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trasicoes_AspNetUsers_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trasicoes_Etapas_EtapaAnteriorlId",
                        column: x => x.EtapaAnteriorlId,
                        principalTable: "Etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trasicoes_Etapas_EtapaAtualId",
                        column: x => x.EtapaAtualId,
                        principalTable: "Etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trasicoes_Ocorrencias_OcorrenciaId",
                        column: x => x.OcorrenciaId,
                        principalTable: "Ocorrencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trasicoes_EtapaAnteriorlId",
                table: "Trasicoes",
                column: "EtapaAnteriorlId");

            migrationBuilder.CreateIndex(
                name: "IX_Trasicoes_EtapaAtualId",
                table: "Trasicoes",
                column: "EtapaAtualId");

            migrationBuilder.CreateIndex(
                name: "IX_Trasicoes_OcorrenciaId",
                table: "Trasicoes",
                column: "OcorrenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Trasicoes_ResponsavelId",
                table: "Trasicoes",
                column: "ResponsavelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trasicoes");

            migrationBuilder.DropColumn(
                name: "Protocolo",
                table: "Ocorrencias");

            migrationBuilder.RenameColumn(
                name: "Campos_TipoDeRisco",
                table: "Ocorrencias",
                newName: "TipoDeRisco");

            migrationBuilder.RenameColumn(
                name: "Campos_TipificacaoDaOcorrencia",
                table: "Ocorrencias",
                newName: "TipificacaoDaOcorrencia");

            migrationBuilder.RenameColumn(
                name: "Campos_SolicitanteRG",
                table: "Ocorrencias",
                newName: "SolicitanteRG");

            migrationBuilder.RenameColumn(
                name: "Campos_SolicitanteNome",
                table: "Ocorrencias",
                newName: "SolicitanteNome");

            migrationBuilder.RenameColumn(
                name: "Campos_SolicitanteCPF",
                table: "Ocorrencias",
                newName: "SolicitanteCPF");

            migrationBuilder.RenameColumn(
                name: "Campos_RegimeDeOcupacaoDoImovel",
                table: "Ocorrencias",
                newName: "RegimeDeOcupacaoDoImovel");

            migrationBuilder.RenameColumn(
                name: "Campos_PossuiUnidadeFamiliar",
                table: "Ocorrencias",
                newName: "PossuiUnidadeFamiliar");

            migrationBuilder.RenameColumn(
                name: "Campos_PossuiIPTU",
                table: "Ocorrencias",
                newName: "PossuiIPTU");

            migrationBuilder.RenameColumn(
                name: "Campos_NumeroDePavimentos",
                table: "Ocorrencias",
                newName: "NumeroDePavimentos");

            migrationBuilder.RenameColumn(
                name: "Campos_NumeroDeMoradias",
                table: "Ocorrencias",
                newName: "NumeroDeMoradias");

            migrationBuilder.RenameColumn(
                name: "Campos_NumeroDeIdosos",
                table: "Ocorrencias",
                newName: "NumeroDeIdosos");

            migrationBuilder.RenameColumn(
                name: "Campos_NumeroDeDeficientes",
                table: "Ocorrencias",
                newName: "NumeroDeDeficientes");

            migrationBuilder.RenameColumn(
                name: "Campos_NumeroDeCriancas",
                table: "Ocorrencias",
                newName: "NumeroDeCriancas");

            migrationBuilder.RenameColumn(
                name: "Campos_NumeroDeComodos",
                table: "Ocorrencias",
                newName: "NumeroDeComodos");

            migrationBuilder.RenameColumn(
                name: "Campos_NumeroDeAdultos",
                table: "Ocorrencias",
                newName: "NumeroDeAdultos");

            migrationBuilder.RenameColumn(
                name: "Campos_Motivacao",
                table: "Ocorrencias",
                newName: "Motivacao");

            migrationBuilder.RenameColumn(
                name: "Campos_Localizacao_Rua",
                table: "Ocorrencias",
                newName: "Localizacao_Rua");

            migrationBuilder.RenameColumn(
                name: "Campos_Localizacao_Numero",
                table: "Ocorrencias",
                newName: "Numero");

            migrationBuilder.RenameColumn(
                name: "Campos_Localizacao_Longitude",
                table: "Ocorrencias",
                newName: "Localizacao_Longitude");

            migrationBuilder.RenameColumn(
                name: "Campos_Localizacao_Latitude",
                table: "Ocorrencias",
                newName: "Localizacao_Latitude");

            migrationBuilder.RenameColumn(
                name: "Campos_Localizacao_Complemento",
                table: "Ocorrencias",
                newName: "Localizacao_Complemento");

            migrationBuilder.RenameColumn(
                name: "Campos_Localizacao_CEP",
                table: "Ocorrencias",
                newName: "Localizacao_CEP");

            migrationBuilder.RenameColumn(
                name: "Campos_Localizacao_Bairro",
                table: "Ocorrencias",
                newName: "Localizacao_Bairro");

            migrationBuilder.RenameColumn(
                name: "Campos_GrauDeRisco",
                table: "Ocorrencias",
                newName: "GrauDeRisco");

            migrationBuilder.RenameColumn(
                name: "Campos_Estrutura",
                table: "Ocorrencias",
                newName: "Estrutura");

            migrationBuilder.RenameColumn(
                name: "Campos_Edificacao",
                table: "Ocorrencias",
                newName: "Edificacao");

            migrationBuilder.RenameColumn(
                name: "Campos_DataEHoraTerminoAtendimento",
                table: "Ocorrencias",
                newName: "DataEHoraTerminoAtendimento");

            migrationBuilder.RenameColumn(
                name: "Campos_DataEHoraInicioAtendimento",
                table: "Ocorrencias",
                newName: "DataEHoraInicioAtendimento");

            migrationBuilder.RenameColumn(
                name: "Campos_DataEHoraDoOcorrido",
                table: "Ocorrencias",
                newName: "DataEHoraDoOcorrido");

            migrationBuilder.RenameColumn(
                name: "Campos_CaracterizacaoDoLocal",
                table: "Ocorrencias",
                newName: "CaracterizacaoDoLocal");

            migrationBuilder.RenameColumn(
                name: "Campos_AreasAfetadas",
                table: "Ocorrencias",
                newName: "AreasAfetadas");

            migrationBuilder.RenameColumn(
                name: "Campos_AnalisePreliminar",
                table: "Ocorrencias",
                newName: "AnalisePreliminar");

            migrationBuilder.RenameColumn(
                name: "isVisivel",
                table: "Ocorrencias",
                newName: "isVisible");

            migrationBuilder.AlterColumn<string>(
                name: "Numero",
                table: "Ocorrencias",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localizacao_Numero",
                table: "Ocorrencias",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoEntidade",
                table: "Anexos",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_EntidadeId_TipoEntidade",
                table: "Anexos",
                columns: new[] { "EntidadeId", "TipoEntidade" });
        }
    }
}
