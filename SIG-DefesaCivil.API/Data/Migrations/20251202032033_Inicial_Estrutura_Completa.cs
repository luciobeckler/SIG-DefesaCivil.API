using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_DefesaCivil.API.Migrations
{
    /// <inheritdoc />
    public partial class Inicial_Estrutura_Completa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isAtivo = table.Column<bool>(type: "bit", nullable: false),
                    isPrimeiroAcesso = table.Column<bool>(type: "bit", nullable: false),
                    DataAdmissao = table.Column<DateOnly>(type: "date", nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataDeNascimento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Civis",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    Rua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CEP = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Naturezas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoNatureza = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NaturezaPaiId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quadros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "Etapas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Posicao = table.Column<int>(type: "int", nullable: false),
                    MinTempoNaEtapa = table.Column<TimeSpan>(type: "time", nullable: true),
                    EtapasDestinoId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermissoesParaTransicionarParaEstaEtapa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuadroId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataEHoraDaSolicitacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataEHoraInicioAtendimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataEHoraTerminoAtendimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SolicitanteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnderecoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AnalisePreliminar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaracterizacaoDoLocal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Edificacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estrutura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoDeRisco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipificacaoDaOcorrencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motivacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AreasAfetadas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PossuiIPTU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroDeMoradias = table.Column<int>(type: "int", nullable: false),
                    NumeroDeComodos = table.Column<int>(type: "int", nullable: false),
                    NumeroDePavimentos = table.Column<int>(type: "int", nullable: false),
                    PossuiUnidadeFamiliar = table.Column<bool>(type: "bit", nullable: false),
                    NumeroDeDeficientes = table.Column<int>(type: "int", nullable: false),
                    NumeroDeCriancas = table.Column<int>(type: "int", nullable: false),
                    NumeroDeAdultos = table.Column<int>(type: "int", nullable: false),
                    NumeroDeIdosos = table.Column<int>(type: "int", nullable: false),
                    GrauDeRisco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegimeDeOcupacaoDoImovel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataEntradaNaFaseAtual = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isVisible = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioCriadorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OcorrenciaPaiId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EtapaId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ocorrencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_AspNetUsers_UsuarioCriadorId",
                        column: x => x.UsuarioCriadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_Civis_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "Civis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Enderecos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_Etapas_EtapaId",
                        column: x => x.EtapaId,
                        principalTable: "Etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_Ocorrencias_OcorrenciaPaiId",
                        column: x => x.OcorrenciaPaiId,
                        principalTable: "Ocorrencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NomeOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlArmazenamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdArquivoExterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoConteudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntidadeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TipoEntidade = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OcorrenciaId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                name: "OcorrenciaNaturezas",
                columns: table => new
                {
                    NaturezasId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OcorrenciasId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcorrenciaNaturezas", x => new { x.NaturezasId, x.OcorrenciasId });
                    table.ForeignKey(
                        name: "FK_OcorrenciaNaturezas_Naturezas_NaturezasId",
                        column: x => x.NaturezasId,
                        principalTable: "Naturezas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OcorrenciaNaturezas_Ocorrencias_OcorrenciasId",
                        column: x => x.OcorrenciasId,
                        principalTable: "Ocorrencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OcorrenciasHistoricos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OcorrenciaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Acao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UltimaAlteracao = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_EntidadeId_TipoEntidade",
                table: "Anexos",
                columns: new[] { "EntidadeId", "TipoEntidade" });

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
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

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
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Etapas_QuadroId",
                table: "Etapas",
                column: "QuadroId");

            migrationBuilder.CreateIndex(
                name: "IX_Naturezas_NaturezaPaiId",
                table: "Naturezas",
                column: "NaturezaPaiId");

            migrationBuilder.CreateIndex(
                name: "IX_OcorrenciaNaturezas_OcorrenciasId",
                table: "OcorrenciaNaturezas",
                column: "OcorrenciasId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_EnderecoId",
                table: "Ocorrencias",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_EtapaId",
                table: "Ocorrencias",
                column: "EtapaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_OcorrenciaPaiId",
                table: "Ocorrencias",
                column: "OcorrenciaPaiId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_SolicitanteId",
                table: "Ocorrencias",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_UsuarioCriadorId",
                table: "Ocorrencias",
                column: "UsuarioCriadorId");

            migrationBuilder.CreateIndex(
                name: "IX_OcorrenciasHistoricos_OcorrenciaId",
                table: "OcorrenciasHistoricos",
                column: "OcorrenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_OcorrenciasHistoricos_UsuarioId",
                table: "OcorrenciasHistoricos",
                column: "UsuarioId");
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
                name: "OcorrenciaNaturezas");

            migrationBuilder.DropTable(
                name: "OcorrenciasHistoricos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Naturezas");

            migrationBuilder.DropTable(
                name: "Ocorrencias");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Civis");

            migrationBuilder.DropTable(
                name: "Enderecos");

            migrationBuilder.DropTable(
                name: "Etapas");

            migrationBuilder.DropTable(
                name: "Quadros");
        }
    }
}
