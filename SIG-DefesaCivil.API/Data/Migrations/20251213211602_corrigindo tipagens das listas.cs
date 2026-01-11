using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class corrigindotipagensdaslistas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ocorrencias_Civis_SolicitanteId",
                table: "Ocorrencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Ocorrencias_Enderecos_EnderecoId",
                table: "Ocorrencias");

            migrationBuilder.DropTable(
                name: "Civis");

            migrationBuilder.DropTable(
                name: "Enderecos");

            migrationBuilder.DropIndex(
                name: "IX_Ocorrencias_EnderecoId",
                table: "Ocorrencias");

            migrationBuilder.DropIndex(
                name: "IX_Ocorrencias_SolicitanteId",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "EnderecoId",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "SolicitanteId",
                table: "Ocorrencias");

            migrationBuilder.AlterColumn<string>(
                name: "TipoDeRisco",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipificacaoDaOcorrencia",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Motivacao",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estrutura",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Edificacao",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEHoraDoOcorrido",
                table: "Ocorrencias",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CaracterizacaoDoLocal",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AreasAfetadas",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AnalisePreliminar",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnderecoBairro",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnderecoCEP",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnderecoComplemento",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnderecoNumero",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnderecoRua",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SolicitanteCPF",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SolicitanteNome",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SolicitanteRG",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnderecoBairro",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "EnderecoCEP",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "EnderecoComplemento",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "EnderecoNumero",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "EnderecoRua",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "SolicitanteCPF",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "SolicitanteNome",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "SolicitanteRG",
                table: "Ocorrencias");

            migrationBuilder.AlterColumn<string>(
                name: "TipoDeRisco",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TipificacaoDaOcorrencia",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Motivacao",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Estrutura",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Edificacao",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEHoraDoOcorrido",
                table: "Ocorrencias",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CaracterizacaoDoLocal",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AreasAfetadas",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AnalisePreliminar",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "EnderecoId",
                table: "Ocorrencias",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SolicitanteId",
                table: "Ocorrencias",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Civis",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RG = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Civis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CEP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rua = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_EnderecoId",
                table: "Ocorrencias",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_SolicitanteId",
                table: "Ocorrencias",
                column: "SolicitanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ocorrencias_Civis_SolicitanteId",
                table: "Ocorrencias",
                column: "SolicitanteId",
                principalTable: "Civis",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ocorrencias_Enderecos_EnderecoId",
                table: "Ocorrencias",
                column: "EnderecoId",
                principalTable: "Enderecos",
                principalColumn: "Id");
        }
    }
}
