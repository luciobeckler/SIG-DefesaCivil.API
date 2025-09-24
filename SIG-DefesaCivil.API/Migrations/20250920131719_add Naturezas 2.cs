using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class addNaturezas2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evento_AspNetUsers_UsuarioId",
                table: "Evento");

            migrationBuilder.DropForeignKey(
                name: "FK_Evento_Evento_EventoPaiId",
                table: "Evento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Evento",
                table: "Evento");

            migrationBuilder.RenameTable(
                name: "Evento",
                newName: "Eventos");

            migrationBuilder.RenameIndex(
                name: "IX_Evento_UsuarioId",
                table: "Eventos",
                newName: "IX_Eventos_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Evento_EventoPaiId",
                table: "Eventos",
                newName: "IX_Eventos_EventoPaiId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Eventos",
                table: "Eventos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_AspNetUsers_UsuarioId",
                table: "Eventos",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Eventos_EventoPaiId",
                table: "Eventos",
                column: "EventoPaiId",
                principalTable: "Eventos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_AspNetUsers_UsuarioId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Eventos_EventoPaiId",
                table: "Eventos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Eventos",
                table: "Eventos");

            migrationBuilder.RenameTable(
                name: "Eventos",
                newName: "Evento");

            migrationBuilder.RenameIndex(
                name: "IX_Eventos_UsuarioId",
                table: "Evento",
                newName: "IX_Evento_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Eventos_EventoPaiId",
                table: "Evento",
                newName: "IX_Evento_EventoPaiId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Evento",
                table: "Evento",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_AspNetUsers_UsuarioId",
                table: "Evento",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_Evento_EventoPaiId",
                table: "Evento",
                column: "EventoPaiId",
                principalTable: "Evento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
