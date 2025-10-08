using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class AlterandonomedascolunasdatabelaEventosparausuarioCriado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_AspNetUsers_UsuarioId",
                table: "Eventos");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Eventos",
                newName: "UsuarioCriadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Eventos_UsuarioId",
                table: "Eventos",
                newName: "IX_Eventos_UsuarioCriadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_AspNetUsers_UsuarioCriadorId",
                table: "Eventos",
                column: "UsuarioCriadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_AspNetUsers_UsuarioCriadorId",
                table: "Eventos");

            migrationBuilder.RenameColumn(
                name: "UsuarioCriadorId",
                table: "Eventos",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Eventos_UsuarioCriadorId",
                table: "Eventos",
                newName: "IX_Eventos_UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_AspNetUsers_UsuarioId",
                table: "Eventos",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
