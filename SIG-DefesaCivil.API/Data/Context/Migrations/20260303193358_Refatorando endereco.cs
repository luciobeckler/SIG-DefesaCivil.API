using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class Refatorandoendereco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnderecoRua",
                table: "Ocorrencias",
                newName: "Localizacao_Rua");

            migrationBuilder.RenameColumn(
                name: "EnderecoNumero",
                table: "Ocorrencias",
                newName: "Localizacao_Numero");

            migrationBuilder.RenameColumn(
                name: "EnderecoComplemento",
                table: "Ocorrencias",
                newName: "Localizacao_Longitude");

            migrationBuilder.RenameColumn(
                name: "EnderecoCEP",
                table: "Ocorrencias",
                newName: "Localizacao_Latitude");

            migrationBuilder.RenameColumn(
                name: "EnderecoBairro",
                table: "Ocorrencias",
                newName: "Localizacao_Complemento");

            migrationBuilder.RenameColumn(
                name: "LongitudeCaptura",
                table: "Anexos",
                newName: "Localizacao_Rua");

            migrationBuilder.RenameColumn(
                name: "LatitudeCaptura",
                table: "Anexos",
                newName: "Localizacao_Numero");

            migrationBuilder.AddColumn<string>(
                name: "Localizacao_Bairro",
                table: "Ocorrencias",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localizacao_CEP",
                table: "Ocorrencias",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localizacao_Bairro",
                table: "Anexos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localizacao_CEP",
                table: "Anexos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localizacao_Complemento",
                table: "Anexos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localizacao_Latitude",
                table: "Anexos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localizacao_Longitude",
                table: "Anexos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Localizacao_Bairro",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "Localizacao_CEP",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "Localizacao_Bairro",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "Localizacao_CEP",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "Localizacao_Complemento",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "Localizacao_Latitude",
                table: "Anexos");

            migrationBuilder.DropColumn(
                name: "Localizacao_Longitude",
                table: "Anexos");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Rua",
                table: "Ocorrencias",
                newName: "EnderecoRua");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Numero",
                table: "Ocorrencias",
                newName: "EnderecoNumero");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Longitude",
                table: "Ocorrencias",
                newName: "EnderecoComplemento");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Latitude",
                table: "Ocorrencias",
                newName: "EnderecoCEP");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Complemento",
                table: "Ocorrencias",
                newName: "EnderecoBairro");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Rua",
                table: "Anexos",
                newName: "LongitudeCaptura");

            migrationBuilder.RenameColumn(
                name: "Localizacao_Numero",
                table: "Anexos",
                newName: "LatitudeCaptura");
        }
    }
}
