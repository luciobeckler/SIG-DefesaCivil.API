using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class Adicionaregrasdetransicaoentreetapas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Etapas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "EtapasDestinoId",
                table: "Etapas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxSegundosNaEtapa",
                table: "Etapas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinSegundosNaEtapa",
                table: "Etapas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermissoesParaTransicionarParaEstaEtapa",
                table: "Etapas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EtapasDestinoId",
                table: "Etapas");

            migrationBuilder.DropColumn(
                name: "MaxSegundosNaEtapa",
                table: "Etapas");

            migrationBuilder.DropColumn(
                name: "MinSegundosNaEtapa",
                table: "Etapas");

            migrationBuilder.DropColumn(
                name: "PermissoesParaTransicionarParaEstaEtapa",
                table: "Etapas");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Etapas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
