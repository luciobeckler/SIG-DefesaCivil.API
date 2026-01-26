using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Constants;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Enums;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Data
{
    public class Seeder
    {
        public static async Task SeedAllAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<DefesaCivilDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Cria o Banco se não existir
            await context.Database.EnsureCreatedAsync();

            // 2. Executa os Seeds em ordem
            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
            BoardAndPhaseSeeder(context);
            await NatureSeeder(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var rolesToCreate = RolePermissions.GetAllRoles();

            foreach (var role in rolesToCreate)
            {
                var roleName = role.ToString();
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<Usuario> userManager)
        {
            // --- Usuário ADMIN ---
            var adminEmail = "admin@teste.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var newAdmin = new Usuario
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nome = "Lúcio Beckler Passos",
                    Telefone = "31985211711",
                    CPF = "14485403645",
                    Cargo = nameof(ECargos.Administrador),
                    DataAdmissao = DateOnly.FromDateTime(DateTime.Now),
                    isAtivo = true,
                    isPrimeiroAcesso = false
                };

                var result = await userManager.CreateAsync(newAdmin, "SenhaForte123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, nameof(ECargos.Administrador));
                }
            }

            // --- Usuário SISTEMA (Worker) ---
            var sistemaEmail = "sistema@admin.com";
            if (await userManager.FindByEmailAsync(sistemaEmail) == null)
            {
                var newSistema = new Usuario
                {
                    UserName = sistemaEmail,
                    Email = sistemaEmail,
                    Nome = "Sistema Automático",
                    Telefone = "00000000000",
                    CPF = "00000000000",
                    Cargo = nameof(ECargos.Administrador),
                    DataAdmissao = DateOnly.FromDateTime(DateTime.Now),
                    isAtivo = true,
                    isPrimeiroAcesso = false
                };

                var result = await userManager.CreateAsync(newSistema, "Sistema#Auto2025!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newSistema, nameof(ECargos.Administrador));
                }
            }
        }

        public static void BoardAndPhaseSeeder(DefesaCivilDbContext context)
        {
            // Garante que o banco existe
            context.Database.EnsureCreated();

            // Verifica se já existem quadros. Se sim, não faz nada.
            if (context.Quadros.Any())
            {
                return;
            }

            // ==============================================================================
            // 1. QUADRO DE RESPOSTA (URGENTE)
            // ==============================================================================
            var quadroUrgente = new Quadro
            {
                Nome = "Quadro de Resposta (Eventos Urgentes)",
                Descricao = "Foco em agilidade e atendimento imediato (SLA 24h)."
            };

            // Criando as etapas (instanciando primeiro para ter os IDs)
            var q1_e1 = new Etapa { Nome = "Em Alerta", Descricao = "Ocorrência registrada, aguardando equipe.", Posicao = 1, QuadroId = quadroUrgente.Id };
            var q1_e2 = new Etapa { Nome = "Em Deslocamento/Atendimento", Descricao = "Equipe em rota ou atuando na mitigação.", Posicao = 2, QuadroId = quadroUrgente.Id };
            var q1_e3 = new Etapa { Nome = "Relatório de Resposta", Descricao = "Emergência contida. Fase de documentação.", Posicao = 3, QuadroId = quadroUrgente.Id };
            var q1_e4 = new Etapa { Nome = "Concluído", Descricao = "Atendimento emergencial finalizado.", Posicao = 4, QuadroId = quadroUrgente.Id };

            // Configurando SLAs (Exemplo: 24h totais, divididas entre as etapas ativas)
            q1_e1.MaxTempoNaEtapa = TimeSpan.FromHours(2);  // 2h para assumir
            q1_e2.MaxTempoNaEtapa = TimeSpan.FromHours(4);  // 4h para conter/chegar
            q1_e3.MaxTempoNaEtapa = TimeSpan.FromHours(18); // Restante para documentar

            // Configurando o Fluxo (Etapas Destino)
            // Alerta -> Deslocamento
            q1_e1.EtapasDestinoId.Add(q1_e2.Id);

            // Deslocamento -> Relatório
            q1_e2.EtapasDestinoId.Add(q1_e3.Id);

            // Relatório -> Concluído
            q1_e3.EtapasDestinoId.Add(q1_e4.Id);

            // Adicionando à lista do quadro
            quadroUrgente.Etapas.AddRange(new[] { q1_e1, q1_e2, q1_e3, q1_e4 });


            // ==============================================================================
            // 2. QUADRO DE VISTORIA/PREVENÇÃO (NÃO URGENTE)
            // ==============================================================================
            var quadroNaoUrgente = new Quadro
            {
                Nome = "Quadro de Vistoria/Prevenção",
                Descricao = "Foco em análise técnica e documentação (SLA 4/5 dias)."
            };

            var q2_e1 = new Etapa { Nome = "Triagem/Pendente", Descricao = "Aguardando agendamento.", Posicao = 1, QuadroId = quadroNaoUrgente.Id };
            var q2_e2 = new Etapa { Nome = "Em Vistoria", Descricao = "Visita técnica em andamento.", Posicao = 2, QuadroId = quadroNaoUrgente.Id };
            var q2_e3 = new Etapa { Nome = "Em Monitoramento", Descricao = "Necessita observação contínua.", Posicao = 3, QuadroId = quadroNaoUrgente.Id };
            var q2_e4 = new Etapa { Nome = "Elaboração de Laudo", Descricao = "Escrita do relatório técnico.", Posicao = 4, QuadroId = quadroNaoUrgente.Id };
            var q2_e5 = new Etapa { Nome = "Revisão Técnica (Agente)", Descricao = "Ajustes finais (24h).", Posicao = 5, QuadroId = quadroNaoUrgente.Id };
            var q2_e6 = new Etapa { Nome = "Homologação (Diretoria)", Descricao = "Validação superior.", Posicao = 6, QuadroId = quadroNaoUrgente.Id };
            var q2_e7 = new Etapa { Nome = "Arquivado/Finalizado", Descricao = "Processo encerrado.", Posicao = 7, QuadroId = quadroNaoUrgente.Id };

            // Configurando SLAs (Exemplo baseados em 5 dias úteis)
            q2_e1.MaxTempoNaEtapa = TimeSpan.FromDays(1);
            q2_e2.MaxTempoNaEtapa = TimeSpan.FromDays(1);
            q2_e4.MaxTempoNaEtapa = TimeSpan.FromDays(2);
            q2_e5.MaxTempoNaEtapa = TimeSpan.FromHours(24); // Explicito no texto

            // Configurando Fluxo
            q2_e1.EtapasDestinoId.Add(q2_e2.Id); // Triagem -> Vistoria

            q2_e2.EtapasDestinoId.Add(q2_e3.Id); // Vistoria -> Monitoramento
            q2_e2.EtapasDestinoId.Add(q2_e4.Id); // Vistoria -> Laudo (Caminho feliz)

            q2_e3.EtapasDestinoId.Add(q2_e4.Id); // Monitoramento -> Laudo

            q2_e4.EtapasDestinoId.Add(q2_e5.Id); // Laudo -> Revisão

            q2_e5.EtapasDestinoId.Add(q2_e6.Id); // Revisão -> Homologação

            q2_e6.EtapasDestinoId.Add(q2_e7.Id); // Homologação -> Finalizado
            q2_e6.EtapasDestinoId.Add(q2_e4.Id); // Homologação -> Laudo (Caso reprovado, volta para correção)

            // Configurando Permissões Específicas
            // Apenas Diretores podem mover PARA a etapa de Finalizado (Aprovando a homologação)
            // Assumindo que você tem ECargos.Diretor
            q2_e6.PermissoesParaTransicionarParaEstaEtapa.Add(ECargos.Diretor);
            q2_e6.PermissoesParaTransicionarParaEstaEtapa.Add(ECargos.Administrador);

            quadroNaoUrgente.Etapas.AddRange(new[] { q2_e1, q2_e2, q2_e3, q2_e4, q2_e5, q2_e6, q2_e7 });

            // ==============================================================================
            // SALVAR NO BANCO
            // ==============================================================================

            // Adicionamos os Quadros (o EF Core identifica as etapas filhas e salva em cascata)
            context.Quadros.AddRange(quadroUrgente, quadroNaoUrgente);

            context.SaveChanges();
        }
        // Método auxiliar para garantir que o código fique limpo
        private static Natureza CreateNature(string nome, string codigo, string? paiId, string? descricao = null)
        {
            return new Natureza
            {
                Id = Guid.NewGuid().ToString(),
                Nome = nome,
                CodigoNatureza = codigo,
                NaturezaPaiId = paiId,
                Descricao = descricao // Opcional, apenas para folhas se quiser exibir no front
            };
        }

        public static async Task NatureSeeder(DefesaCivilDbContext context)
        {
            // Se já existem naturezas, não faz nada (evita duplicidade)
            if (await context.Naturezas.AnyAsync()) return;

            // =================================================================================
            // 1. NATURAIS
            // =================================================================================
            var naturais = CreateNature("Naturais", "1.0.0.0.0", null);
            context.Naturezas.Add(naturais);
            await context.SaveChangesAsync();

            // ---------------------------------------------------------------------------------
            // 1.1 GEOLÓGICO
            // ---------------------------------------------------------------------------------
            var geologico = CreateNature("Geológico", "1.1.0.0.0", naturais.Id);
            context.Naturezas.Add(geologico);
            await context.SaveChangesAsync();

            // 1.1.1 Terremoto
            var terremoto = CreateNature("Terremoto", "1.1.1.0.0", geologico.Id);
            context.Naturezas.Add(terremoto);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Tremor de terra", "1.1.1.1.0", terremoto.Id, "Vibrações do terreno que provocam oscilações verticais e horizontais na superfície da Terra (ondas sísmicas). Pode ser natural (tectônica) ou induzido."));
            context.Naturezas.Add(CreateNature("Tsunami", "1.1.1.2.0", terremoto.Id, "Série de ondas geradas por deslocamento de um grande volume de água causado geralmente por terremotos, erupções vulcânicas ou movimentos de massa."));

            // 1.1.2 Emanação Vulcânica
            context.Naturezas.Add(CreateNature("Emanação vulcânica", "1.1.2.0.0", geologico.Id, "Produtos/materiais vulcânicos lançados na atmosfera a partir de erupções vulcânicas."));

            // 1.1.3 Movimento de Massa
            var movMassa = CreateNature("Movimento de massa", "1.1.3.0.0", geologico.Id);
            context.Naturezas.Add(movMassa);
            await context.SaveChangesAsync();

            // 1.1.3.1 Quedas, tombamentos e rolamentos
            var quedas = CreateNature("Quedas, tombamentos e rolamentos", "1.1.3.1.0", movMassa.Id);
            context.Naturezas.Add(quedas);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Blocos", "1.1.3.1.1", quedas.Id, "Quedas de blocos (movimento rápido, queda livre), Tombamentos (rotação) ou Rolamentos (movimento ao longo da encosta)."));
            context.Naturezas.Add(CreateNature("Lascas", "1.1.3.1.2", quedas.Id, "Quedas de fatias delgadas formadas pelos fragmentos de rochas."));
            context.Naturezas.Add(CreateNature("Matacães", "1.1.3.1.3", quedas.Id, "Rolamentos rápidos de materiais rochosos diversos e de volumes variáveis em plano inclinado."));
            context.Naturezas.Add(CreateNature("Lajes", "1.1.3.1.4", quedas.Id, "Quedas de fragmentos de rochas extensas de superfície mais ou menos plana e de pouca espessura."));

            // 1.1.3.2 Deslizamentos
            var deslizamentos = CreateNature("Deslizamentos", "1.1.3.2.0", movMassa.Id);
            context.Naturezas.Add(deslizamentos);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Deslizamentos de solo e/ou rocha", "1.1.3.2.1", deslizamentos.Id, "Movimentos rápidos de solo ou rocha com superfície de ruptura bem definida."));

            // 1.1.3.3 Corridas de Massa
            var corridas = CreateNature("Corridas de massa", "1.1.3.3.0", movMassa.Id);
            context.Naturezas.Add(corridas);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Solo/Lama", "1.1.3.3.1", corridas.Id, "Solo/lama misturado com água, com comportamento de líquido viscoso."));
            context.Naturezas.Add(CreateNature("Rocha/Detrito", "1.1.3.3.2", corridas.Id, "Rocha/detrito misturado com água, com comportamento de líquido viscoso."));

            // 1.1.3.4 Subsidências e colapsos
            context.Naturezas.Add(CreateNature("Subsidências e colapsos", "1.1.3.4.0", movMassa.Id, "Afundamento rápido ou gradual do terreno devido ao colapso de cavidades ou deformação do solo."));

            // 1.1.4 Erosão
            var erosao = CreateNature("Erosão", "1.1.4.0.0", geologico.Id);
            context.Naturezas.Add(erosao);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Erosão costeira/Marinha", "1.1.4.1.0", erosao.Id, "Processo de desgaste ao longo da linha da costa devido à ação das ondas e marés."));
            context.Naturezas.Add(CreateNature("Erosão de margem fluvial", "1.1.4.2.0", erosao.Id, "Desgaste das encostas dos rios que provoca desmoronamento de barrancos."));

            var erosaoContinental = CreateNature("Erosão continental", "1.1.4.3.0", erosao.Id);
            context.Naturezas.Add(erosaoContinental);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Laminar", "1.1.4.3.1", erosaoContinental.Id, "Remoção de camada delgada do solo superficial."));
            context.Naturezas.Add(CreateNature("Ravinas", "1.1.4.3.2", erosaoContinental.Id, "Evolução da desagregação do solo provocada por escoamento superficial concentrado."));
            context.Naturezas.Add(CreateNature("Boçorocas", "1.1.4.3.3", erosaoContinental.Id, "Evolução do ravinamento, afetando também o escoamento subsuperficial (freático)."));

            // ---------------------------------------------------------------------------------
            // 1.2 HIDROLÓGICO
            // ---------------------------------------------------------------------------------
            var hidrologico = CreateNature("Hidrológico", "1.2.0.0.0", naturais.Id);
            context.Naturezas.Add(hidrologico);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Inundações", "1.2.1.0.0", hidrologico.Id, "Submersão gradual de áreas fora dos limites normais de um curso de água (planícies)."));
            context.Naturezas.Add(CreateNature("Enxurradas", "1.2.2.0.0", hidrologico.Id, "Escoamento superficial de alta velocidade e energia (Flash Floods)."));
            context.Naturezas.Add(CreateNature("Alagamentos", "1.2.3.0.0", hidrologico.Id, "Acúmulo de água em áreas urbanas por deficiência de drenagem."));

            // ---------------------------------------------------------------------------------
            // 1.3 METEOROLÓGICO
            // ---------------------------------------------------------------------------------
            var meteorologico = CreateNature("Meteorológico", "1.3.0.0.0", naturais.Id);
            context.Naturezas.Add(meteorologico);
            await context.SaveChangesAsync();

            // 1.3.1 Sistemas de Grande Escala
            var sistGrandeEscala = CreateNature("Sistemas de grande escala/Escala regional", "1.3.1.0.0", meteorologico.Id);
            context.Naturezas.Add(sistGrandeEscala);
            await context.SaveChangesAsync();

            var ciclones = CreateNature("Ciclones", "1.3.1.1.0", sistGrandeEscala.Id);
            context.Naturezas.Add(ciclones);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Ventos costeiros (mobilidade de dunas)", "1.3.1.1.1", ciclones.Id, "Movimentação de dunas de areia sobre construções na orla."));
            context.Naturezas.Add(CreateNature("Marés de tempestade (ressaca)", "1.3.1.1.2", ciclones.Id, "Ondas violentas que geram maior agitação do mar e elevação do nível do oceano."));

            context.Naturezas.Add(CreateNature("Frentes frias/Zonas de convergência", "1.3.1.2.0", sistGrandeEscala.Id, "Avanço de massa de ar frio ou zonas de baixa pressão provocando chuvas e ventos."));

            // 1.3.2 Tempestades
            var tempestades = CreateNature("Tempestades", "1.3.2.0.0", meteorologico.Id);
            context.Naturezas.Add(tempestades);
            await context.SaveChangesAsync();

            var tempestadeLocal = CreateNature("Tempestade local/Convectiva", "1.3.2.1.0", tempestades.Id);
            context.Naturezas.Add(tempestadeLocal);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Tornados", "1.3.2.1.1", tempestadeLocal.Id, "Coluna de ar giratória violenta em contato com a terra."));
            context.Naturezas.Add(CreateNature("Tempestade de raios", "1.3.2.1.2", tempestadeLocal.Id, "Tempestade com intensa atividade elétrica."));
            context.Naturezas.Add(CreateNature("Granizo", "1.3.2.1.3", tempestadeLocal.Id, "Precipitação de pedaços irregulares de gelo."));
            context.Naturezas.Add(CreateNature("Chuvas intensas", "1.3.2.1.4", tempestadeLocal.Id, "Chuvas com acumulados significativos causando múltiplos desastres."));
            context.Naturezas.Add(CreateNature("Vendaval", "1.3.2.1.5", tempestadeLocal.Id, "Forte deslocamento de uma massa de ar."));

            // 1.3.3 Temperaturas Extremas
            var tempExtremas = CreateNature("Temperaturas extremas", "1.3.3.0.0", meteorologico.Id);
            context.Naturezas.Add(tempExtremas);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Onda de calor", "1.3.3.1.0", tempExtremas.Id, "Período prolongado excessivamente quente (mín 3 dias, 5°C acima da média)."));

            var ondaFrio = CreateNature("Onda de frio", "1.3.3.2.0", tempExtremas.Id);
            context.Naturezas.Add(ondaFrio);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Friagem", "1.3.3.2.1", ondaFrio.Id, "Queda de temperatura mínima abaixo do esperado por 3 a 4 dias."));
            context.Naturezas.Add(CreateNature("Geadas", "1.3.3.2.2", ondaFrio.Id, "Formação de camada de cristais de gelo na superfície."));

            // ---------------------------------------------------------------------------------
            // 1.4 CLIMATOLÓGICO
            // ---------------------------------------------------------------------------------
            var climatologico = CreateNature("Climatológico", "1.4.0.0.0", naturais.Id);
            context.Naturezas.Add(climatologico);
            await context.SaveChangesAsync();

            // 1.4.1 Seca
            var secaGroup = CreateNature("Seca", "1.4.1.0.0", climatologico.Id); // Note que o documento agrupa Estiagem/Seca/Incendio sob 1.4.1 no COBRADE
            context.Naturezas.Add(secaGroup);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Estiagem", "1.4.1.1.0", secaGroup.Id, "Período prolongado de baixa pluviosidade (perda de umidade > reposição)."));
            context.Naturezas.Add(CreateNature("Seca", "1.4.1.2.0", secaGroup.Id, "Estiagem prolongada causando grave desequilíbrio hidrológico."));

            var incendio = CreateNature("Incêndio florestal", "1.4.1.3.0", secaGroup.Id);
            context.Naturezas.Add(incendio);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Incêndios em áreas protegidas", "1.4.1.3.1", incendio.Id, "Fogo sem controle em parques e áreas de preservação."));
            context.Naturezas.Add(CreateNature("Incêndios em áreas não protegidas", "1.4.1.3.2", incendio.Id, "Fogo sem controle em vegetação não protegida, afetando qualidade do ar."));

            context.Naturezas.Add(CreateNature("Baixa umidade do ar", "1.4.1.4.0", secaGroup.Id, "Queda da taxa de vapor de água para abaixo de 20%."));

            // ---------------------------------------------------------------------------------
            // 1.5 BIOLÓGICO
            // ---------------------------------------------------------------------------------
            var biologico = CreateNature("Biológico", "1.5.0.0.0", naturais.Id);
            context.Naturezas.Add(biologico);
            await context.SaveChangesAsync();

            // 1.5.1 Epidemias
            var epidemias = CreateNature("Epidemias", "1.5.1.0.0", biologico.Id);
            context.Naturezas.Add(epidemias);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Doenças infecciosas virais", "1.5.1.1.0", epidemias.Id, "Aumento brusco de doenças geradas por vírus."));
            context.Naturezas.Add(CreateNature("Doenças infecciosas bacterianas", "1.5.1.2.0", epidemias.Id, "Aumento brusco de doenças geradas por bactérias."));
            context.Naturezas.Add(CreateNature("Doenças infecciosas parasíticas", "1.5.1.3.0", epidemias.Id, "Aumento brusco de doenças geradas por parasitas."));
            context.Naturezas.Add(CreateNature("Doenças infecciosas fúngicas", "1.5.1.4.0", epidemias.Id, "Aumento brusco de doenças geradas por fungos."));

            // 1.5.2 Infestações/Pragas
            var pragas = CreateNature("Infestações/Pragas", "1.5.2.0.0", biologico.Id);
            context.Naturezas.Add(pragas);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Infestações de animais", "1.5.2.1.0", pragas.Id, "Animais que alteram o equilíbrio ecológico."));

            var algas = CreateNature("Infestações de algas", "1.5.2.2.0", pragas.Id);
            context.Naturezas.Add(algas);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Marés vermelhas", "1.5.2.2.1", algas.Id, "Aglomeração de microalgas mudando a cor da água."));
            context.Naturezas.Add(CreateNature("Cianobactérias em reservatórios", "1.5.2.2.2", algas.Id, "Aglomeração em reservatórios receptores de dejetos."));

            context.Naturezas.Add(CreateNature("Outras infestações", "1.5.2.3.0", pragas.Id, "Outras infestações que alterem o equilíbrio ecológico."));


            // =================================================================================
            // 2. TECNOLÓGICOS
            // =================================================================================
            var tecnologicos = CreateNature("Tecnológicos", "2.0.0.0.0", null);
            context.Naturezas.Add(tecnologicos);
            await context.SaveChangesAsync();

            // ---------------------------------------------------------------------------------
            // 2.1 SUBSTÂNCIAS RADIOATIVAS
            // ---------------------------------------------------------------------------------
            var radioativos = CreateNature("Desastres relacionados a substâncias radioativas", "2.1.0.0.0", tecnologicos.Id);
            context.Naturezas.Add(radioativos);
            await context.SaveChangesAsync();

            var siderais = CreateNature("Desastres siderais com riscos radioativos", "2.1.1.0.0", radioativos.Id);
            context.Naturezas.Add(siderais);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Queda de satélite (radionuclídeos)", "2.1.1.1.0", siderais.Id, "Queda de satélites com material radioativo."));

            var equipRadioativos = CreateNature("Desastres com substâncias e equipamentos radioativos", "2.1.2.0.0", radioativos.Id);
            context.Naturezas.Add(equipRadioativos);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Fontes radioativas em processos de produção", "2.1.2.1.0", equipRadioativos.Id, "Escapamento acidental de radiação em indústrias/pesquisas."));

            var poluicaoRad = CreateNature("Riscos de intensa poluição ambiental por resíduos radioativos", "2.1.3.0.0", radioativos.Id);
            context.Naturezas.Add(poluicaoRad);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Outras fontes de liberação de radionuclídeos", "2.1.3.1.0", poluicaoRad.Id, "Escapamento de radiação de fontes diversas."));

            // ---------------------------------------------------------------------------------
            // 2.2 PRODUTOS PERIGOSOS
            // ---------------------------------------------------------------------------------
            var prodPerigosos = CreateNature("Desastres relacionados a produtos perigosos", "2.2.0.0.0", tecnologicos.Id);
            context.Naturezas.Add(prodPerigosos);
            await context.SaveChangesAsync();

            var industriasPP = CreateNature("Desastres em plantas/distritos industriais com produtos perigosos", "2.2.1.0.0", prodPerigosos.Id);
            context.Naturezas.Add(industriasPP);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Liberação de produtos químicos para atmosfera", "2.2.1.1.0", industriasPP.Id, "Explosão ou incêndio liberando químicos."));

            var contaminacaoAgua = CreateNature("Desastres relacionados à contaminação da água", "2.2.2.0.0", prodPerigosos.Id);
            context.Naturezas.Add(contaminacaoAgua);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Liberação de químicos em água potável", "2.2.2.1.0", contaminacaoAgua.Id, "Derramamento em sistema de abastecimento."));
            context.Naturezas.Add(CreateNature("Derramamento em ambiente lacustre/fluvial/marinho", "2.2.2.2.0", contaminacaoAgua.Id, "Derramamento em rios, lagos ou mar."));

            var conflitosBelicos = CreateNature("Desastres relacionados a conflitos bélicos", "2.2.3.0.0", prodPerigosos.Id);
            context.Naturezas.Add(conflitosBelicos);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Ações militares (químico/biológico/radiológico)", "2.2.3.1.0", conflitosBelicos.Id, "Agentes perigosos usados em atentados ou guerra."));

            var transportePP = CreateNature("Transporte de produtos perigosos", "2.2.4.0.0", prodPerigosos.Id);
            context.Naturezas.Add(transportePP);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Transporte rodoviário", "2.2.4.1.0", transportePP.Id, "Extravasamento no modal rodoviário."));
            context.Naturezas.Add(CreateNature("Transporte ferroviário", "2.2.4.2.0", transportePP.Id, "Extravasamento no modal ferroviário."));
            context.Naturezas.Add(CreateNature("Transporte aéreo", "2.2.4.3.0", transportePP.Id, "Extravasamento no modal aéreo."));
            context.Naturezas.Add(CreateNature("Transporte dutoviário", "2.2.4.4.0", transportePP.Id, "Extravasamento no modal dutoviário."));
            context.Naturezas.Add(CreateNature("Transporte marítimo", "2.2.4.5.0", transportePP.Id, "Extravasamento no modal marítimo."));
            context.Naturezas.Add(CreateNature("Transporte aquaviário", "2.2.4.6.0", transportePP.Id, "Extravasamento no modal aquaviário."));


            // ---------------------------------------------------------------------------------
            // 2.3 INCÊNDIOS URBANOS
            // ---------------------------------------------------------------------------------
            var incendiosUrbanos = CreateNature("Desastres relacionados a incêndios urbanos", "2.3.0.0.0", tecnologicos.Id);
            context.Naturezas.Add(incendiosUrbanos);
            await context.SaveChangesAsync();

            var incendiosUrbTipos = CreateNature("Incêndios urbanos", "2.3.1.0.0", incendiosUrbanos.Id); // Subgrupo redundante no nome, mas existe na hierarquia
            context.Naturezas.Add(incendiosUrbTipos);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Incêndios em plantas industriais/depósitos", "2.3.1.1.0", incendiosUrbTipos.Id, "Propagação descontrolada do fogo em indústrias/depósitos."));
            context.Naturezas.Add(CreateNature("Incêndios em aglomerados residenciais", "2.3.1.2.0", incendiosUrbTipos.Id, "Propagação descontrolada do fogo em conjuntos habitacionais."));

            // ---------------------------------------------------------------------------------
            // 2.4 OBRAS CIVIS
            // ---------------------------------------------------------------------------------
            var obrasCivis = CreateNature("Desastres relacionados a obras civis", "2.4.0.0.0", tecnologicos.Id);
            context.Naturezas.Add(obrasCivis);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Colapso de edificações", "2.4.1.0.0", obrasCivis.Id, "Queda de estrutura civil."));
            context.Naturezas.Add(CreateNature("Rompimento/colapso de barragens", "2.4.2.0.0", obrasCivis.Id, "Rompimento ou colapso de barragens."));

            // ---------------------------------------------------------------------------------
            // 2.5 TRANSPORTE DE PASSAGEIROS E CARGAS NÃO PERIGOSAS
            // ---------------------------------------------------------------------------------
            var transporteNaoPerigoso = CreateNature("Transporte de passageiros e cargas não perigosas", "2.5.0.0.0", tecnologicos.Id);
            context.Naturezas.Add(transporteNaoPerigoso);
            await context.SaveChangesAsync();
            context.Naturezas.Add(CreateNature("Transporte rodoviário", "2.5.1.0.0", transporteNaoPerigoso.Id, "Acidente rodoviário (passageiros/cargas comuns)."));
            context.Naturezas.Add(CreateNature("Transporte ferroviário", "2.5.2.0.0", transporteNaoPerigoso.Id, "Acidente ferroviário (passageiros/cargas comuns)."));
            context.Naturezas.Add(CreateNature("Transporte aéreo", "2.5.3.0.0", transporteNaoPerigoso.Id, "Acidente aéreo (passageiros/cargas comuns)."));
            context.Naturezas.Add(CreateNature("Transporte marítimo", "2.5.4.0.0", transporteNaoPerigoso.Id, "Acidente marítimo (passageiros/cargas comuns)."));
            context.Naturezas.Add(CreateNature("Transporte aquaviário", "2.5.5.0.0", transporteNaoPerigoso.Id, "Acidente aquaviário (passageiros/cargas comuns)."));

            await context.SaveChangesAsync();
        }
    }
}
