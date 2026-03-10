using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    isAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    isPrimeiroAcesso = table.Column<bool>(type: "boolean", nullable: false),
                    DataAdmissao = table.Column<DateOnly>(type: "date", nullable: true),
                    Endereco = table.Column<string>(type: "text", nullable: true),
                    DataDeNascimento = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Naturezas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    CodigoNatureza = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    NaturezaPaiId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Naturezas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Naturezas_Naturezas_NaturezaPaiId",
                        column: x => x.NaturezaPaiId,
                        principalTable: "Naturezas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quadros",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quadros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Revoked = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Etapas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Posicao = table.Column<int>(type: "integer", nullable: false),
                    MinTempoNaEtapa = table.Column<TimeSpan>(type: "interval", nullable: true),
                    MaxTempoNaEtapa = table.Column<TimeSpan>(type: "interval", nullable: true),
                    EtapasDestinoId = table.Column<List<string>>(type: "text[]", nullable: true),
                    PermissoesParaTransicionarParaEstaEtapa = table.Column<string>(type: "text", nullable: false),
                    QuadroId = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Ocorrencias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Protocolo = table.Column<string>(type: "text", nullable: false),
                    isVisivel = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Campos_DataEHoraDoOcorrido = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Campos_DataEHoraInicioAtendimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Campos_DataEHoraTerminoAtendimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Campos_Localizacao_Rua = table.Column<string>(type: "text", nullable: true),
                    Campos_Localizacao_Numero = table.Column<string>(type: "text", nullable: true),
                    Campos_Localizacao_Complemento = table.Column<string>(type: "text", nullable: true),
                    Campos_Localizacao_Bairro = table.Column<string>(type: "text", nullable: true),
                    Campos_Localizacao_CEP = table.Column<string>(type: "text", nullable: true),
                    Campos_Localizacao_Latitude = table.Column<string>(type: "text", nullable: true),
                    Campos_Localizacao_Longitude = table.Column<string>(type: "text", nullable: true),
                    Campos_Solicitante_Nome = table.Column<string>(type: "text", nullable: true),
                    Campos_Solicitante_CPF = table.Column<string>(type: "text", nullable: true),
                    Campos_Solicitante_Email = table.Column<string>(type: "text", nullable: true),
                    Campos_Solicitante_Telefone = table.Column<string>(type: "text", nullable: true),
                    Campos_AnalisePreliminar = table.Column<string>(type: "text", nullable: false),
                    Campos_CaracterizacaoDoLocal = table.Column<string>(type: "text", nullable: false),
                    Campos_Edificacao = table.Column<string>(type: "text", nullable: false),
                    Campos_Estrutura = table.Column<string>(type: "text", nullable: false),
                    Campos_TipoDeRisco = table.Column<string>(type: "text", nullable: false),
                    Campos_TipificacaoDaOcorrencia = table.Column<string>(type: "text", nullable: false),
                    Campos_Motivacao = table.Column<string>(type: "text", nullable: false),
                    Campos_AreasAfetadas = table.Column<string>(type: "text", nullable: false),
                    Campos_GrauDeRisco = table.Column<string>(type: "text", nullable: true),
                    Campos_RegimeDeOcupacaoDoImovel = table.Column<string>(type: "text", nullable: true),
                    Campos_PossuiIPTU = table.Column<string>(type: "text", nullable: true),
                    Campos_NumeroDeMoradias = table.Column<int>(type: "integer", nullable: true),
                    Campos_NumeroDeComodos = table.Column<int>(type: "integer", nullable: true),
                    Campos_NumeroDePavimentos = table.Column<int>(type: "integer", nullable: true),
                    Campos_PossuiUnidadeFamiliar = table.Column<bool>(type: "boolean", nullable: true),
                    Campos_NumeroDeDeficientes = table.Column<int>(type: "integer", nullable: true),
                    Campos_NumeroDeCriancas = table.Column<int>(type: "integer", nullable: true),
                    Campos_NumeroDeAdultos = table.Column<int>(type: "integer", nullable: true),
                    Campos_NumeroDeIdosos = table.Column<int>(type: "integer", nullable: true),
                    DataEntradaNaFaseAtual = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ResponsavelId = table.Column<string>(type: "text", nullable: true),
                    EtapaId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ocorrencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_AspNetUsers_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_Etapas_EtapaId",
                        column: x => x.EtapaId,
                        principalTable: "Etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UrlArmazenamento = table.Column<string>(type: "text", nullable: false),
                    IdAnexoExterno = table.Column<string>(type: "text", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EntidadeId = table.Column<string>(type: "text", nullable: false),
                    TipoEntidade = table.Column<int>(type: "integer", nullable: false),
                    Localizacao_Rua = table.Column<string>(type: "text", nullable: true),
                    Localizacao_Numero = table.Column<string>(type: "text", nullable: true),
                    Localizacao_Complemento = table.Column<string>(type: "text", nullable: true),
                    Localizacao_Bairro = table.Column<string>(type: "text", nullable: true),
                    Localizacao_CEP = table.Column<string>(type: "text", nullable: true),
                    Localizacao_Latitude = table.Column<string>(type: "text", nullable: true),
                    Localizacao_Longitude = table.Column<string>(type: "text", nullable: true),
                    DataHoraCaptura = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OcorrenciaId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Anexos_Ocorrencias_OcorrenciaId",
                        column: x => x.OcorrenciaId,
                        principalTable: "Ocorrencias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OcorrenciasHistoricos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Acao = table.Column<string>(type: "text", nullable: false),
                    Horarios = table.Column<List<DateTime>>(type: "timestamp without time zone[]", nullable: false),
                    OcorrenciaId = table.Column<string>(type: "text", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcorrenciasHistoricos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OcorrenciasHistoricos_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OcorrenciasHistoricos_Ocorrencias_OcorrenciaId",
                        column: x => x.OcorrenciaId,
                        principalTable: "Ocorrencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trasicoes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DataEHorario = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OcorrenciaId = table.Column<string>(type: "text", nullable: false),
                    ResponsavelId = table.Column<string>(type: "text", nullable: false),
                    EtapaAtualId = table.Column<string>(type: "text", nullable: false),
                    EtapaAnteriorId = table.Column<string>(type: "text", nullable: false)
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
                        name: "FK_Trasicoes_Etapas_EtapaAnteriorId",
                        column: x => x.EtapaAnteriorId,
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
                name: "IX_Anexos_OcorrenciaId",
                table: "Anexos",
                column: "OcorrenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Etapas_QuadroId",
                table: "Etapas",
                column: "QuadroId");

            migrationBuilder.CreateIndex(
                name: "IX_Naturezas_NaturezaPaiId",
                table: "Naturezas",
                column: "NaturezaPaiId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_EtapaId",
                table: "Ocorrencias",
                column: "EtapaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_ResponsavelId",
                table: "Ocorrencias",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_OcorrenciasHistoricos_OcorrenciaId",
                table: "OcorrenciasHistoricos",
                column: "OcorrenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_OcorrenciasHistoricos_UsuarioId",
                table: "OcorrenciasHistoricos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trasicoes_EtapaAnteriorId",
                table: "Trasicoes",
                column: "EtapaAnteriorId");

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
                name: "Anexos");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Naturezas");

            migrationBuilder.DropTable(
                name: "OcorrenciasHistoricos");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Trasicoes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Ocorrencias");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Etapas");

            migrationBuilder.DropTable(
                name: "Quadros");
        }
    }
}
