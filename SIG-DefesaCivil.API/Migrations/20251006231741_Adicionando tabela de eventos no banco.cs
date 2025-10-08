using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class Adicionandotabeladeeventosnobanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Natureza_Natureza_NaturezaPaiId",
                table: "Natureza");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Natureza",
                table: "Natureza");

            migrationBuilder.RenameTable(
                name: "Natureza",
                newName: "Naturezas");

            migrationBuilder.RenameIndex(
                name: "IX_Natureza_NaturezaPaiId",
                table: "Naturezas",
                newName: "IX_Naturezas_NaturezaPaiId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Naturezas",
                table: "Naturezas",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EventosHistoricos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Acao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosHistoricos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventosHistoricos_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventosHistoricos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventosHistoricos_EventoId",
                table: "EventosHistoricos",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosHistoricos_UsuarioId",
                table: "EventosHistoricos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Naturezas_Naturezas_NaturezaPaiId",
                table: "Naturezas",
                column: "NaturezaPaiId",
                principalTable: "Naturezas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Naturezas_Naturezas_NaturezaPaiId",
                table: "Naturezas");

            migrationBuilder.DropTable(
                name: "EventosHistoricos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Naturezas",
                table: "Naturezas");

            migrationBuilder.RenameTable(
                name: "Naturezas",
                newName: "Natureza");

            migrationBuilder.RenameIndex(
                name: "IX_Naturezas_NaturezaPaiId",
                table: "Natureza",
                newName: "IX_Natureza_NaturezaPaiId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Natureza",
                table: "Natureza",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Natureza_Natureza_NaturezaPaiId",
                table: "Natureza",
                column: "NaturezaPaiId",
                principalTable: "Natureza",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
