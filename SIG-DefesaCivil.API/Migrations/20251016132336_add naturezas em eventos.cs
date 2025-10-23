using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class addnaturezasemeventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventoNaturezas",
                columns: table => new
                {
                    EventosId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NaturezasId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoNaturezas", x => new { x.EventosId, x.NaturezasId });
                    table.ForeignKey(
                        name: "FK_EventoNaturezas_Eventos_EventosId",
                        column: x => x.EventosId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoNaturezas_Naturezas_NaturezasId",
                        column: x => x.NaturezasId,
                        principalTable: "Naturezas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoNaturezas_NaturezasId",
                table: "EventoNaturezas",
                column: "NaturezasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoNaturezas");
        }
    }
}
