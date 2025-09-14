using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoisPrimeiroAcessoaomodeldeusuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPrimeiroAcesso",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPrimeiroAcesso",
                table: "AspNetUsers");
        }
    }
}
