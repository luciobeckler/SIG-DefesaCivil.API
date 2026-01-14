using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Models;

public static class CobradeSeeder
{
    // Método auxiliar para garantir que o código fique limpo
    private static Natureza Create(string nome, string codigo, string? paiId, string? descricao = null)
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

    public static async Task Seed(DefesaCivilDbContext context)
    {
        // Se já existem naturezas, não faz nada (evita duplicidade)
        if (await context.Naturezas.AnyAsync()) return;

        // =================================================================================
        // 1. NATURAIS
        // =================================================================================
        var naturais = Create("Naturais", "1.0.0.0.0", null);
        context.Naturezas.Add(naturais);
        await context.SaveChangesAsync();

        // ---------------------------------------------------------------------------------
        // 1.1 GEOLÓGICO
        // ---------------------------------------------------------------------------------
        var geologico = Create("Geológico", "1.1.0.0.0", naturais.Id);
        context.Naturezas.Add(geologico);
        await context.SaveChangesAsync();

        // 1.1.1 Terremoto
        var terremoto = Create("Terremoto", "1.1.1.0.0", geologico.Id);
        context.Naturezas.Add(terremoto);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Tremor de terra", "1.1.1.1.0", terremoto.Id, "Vibrações do terreno que provocam oscilações verticais e horizontais na superfície da Terra (ondas sísmicas). Pode ser natural (tectônica) ou induzido."));
        context.Naturezas.Add(Create("Tsunami", "1.1.1.2.0", terremoto.Id, "Série de ondas geradas por deslocamento de um grande volume de água causado geralmente por terremotos, erupções vulcânicas ou movimentos de massa."));

        // 1.1.2 Emanação Vulcânica
        context.Naturezas.Add(Create("Emanação vulcânica", "1.1.2.0.0", geologico.Id, "Produtos/materiais vulcânicos lançados na atmosfera a partir de erupções vulcânicas."));

        // 1.1.3 Movimento de Massa
        var movMassa = Create("Movimento de massa", "1.1.3.0.0", geologico.Id);
        context.Naturezas.Add(movMassa);
        await context.SaveChangesAsync();

        // 1.1.3.1 Quedas, tombamentos e rolamentos
        var quedas = Create("Quedas, tombamentos e rolamentos", "1.1.3.1.0", movMassa.Id);
        context.Naturezas.Add(quedas);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Blocos", "1.1.3.1.1", quedas.Id, "Quedas de blocos (movimento rápido, queda livre), Tombamentos (rotação) ou Rolamentos (movimento ao longo da encosta)."));
        context.Naturezas.Add(Create("Lascas", "1.1.3.1.2", quedas.Id, "Quedas de fatias delgadas formadas pelos fragmentos de rochas."));
        context.Naturezas.Add(Create("Matacães", "1.1.3.1.3", quedas.Id, "Rolamentos rápidos de materiais rochosos diversos e de volumes variáveis em plano inclinado."));
        context.Naturezas.Add(Create("Lajes", "1.1.3.1.4", quedas.Id, "Quedas de fragmentos de rochas extensas de superfície mais ou menos plana e de pouca espessura."));

        // 1.1.3.2 Deslizamentos
        var deslizamentos = Create("Deslizamentos", "1.1.3.2.0", movMassa.Id);
        context.Naturezas.Add(deslizamentos);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Deslizamentos de solo e/ou rocha", "1.1.3.2.1", deslizamentos.Id, "Movimentos rápidos de solo ou rocha com superfície de ruptura bem definida."));

        // 1.1.3.3 Corridas de Massa
        var corridas = Create("Corridas de massa", "1.1.3.3.0", movMassa.Id);
        context.Naturezas.Add(corridas);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Solo/Lama", "1.1.3.3.1", corridas.Id, "Solo/lama misturado com água, com comportamento de líquido viscoso."));
        context.Naturezas.Add(Create("Rocha/Detrito", "1.1.3.3.2", corridas.Id, "Rocha/detrito misturado com água, com comportamento de líquido viscoso."));

        // 1.1.3.4 Subsidências e colapsos
        context.Naturezas.Add(Create("Subsidências e colapsos", "1.1.3.4.0", movMassa.Id, "Afundamento rápido ou gradual do terreno devido ao colapso de cavidades ou deformação do solo."));

        // 1.1.4 Erosão
        var erosao = Create("Erosão", "1.1.4.0.0", geologico.Id);
        context.Naturezas.Add(erosao);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Erosão costeira/Marinha", "1.1.4.1.0", erosao.Id, "Processo de desgaste ao longo da linha da costa devido à ação das ondas e marés."));
        context.Naturezas.Add(Create("Erosão de margem fluvial", "1.1.4.2.0", erosao.Id, "Desgaste das encostas dos rios que provoca desmoronamento de barrancos."));

        var erosaoContinental = Create("Erosão continental", "1.1.4.3.0", erosao.Id);
        context.Naturezas.Add(erosaoContinental);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Laminar", "1.1.4.3.1", erosaoContinental.Id, "Remoção de camada delgada do solo superficial."));
        context.Naturezas.Add(Create("Ravinas", "1.1.4.3.2", erosaoContinental.Id, "Evolução da desagregação do solo provocada por escoamento superficial concentrado."));
        context.Naturezas.Add(Create("Boçorocas", "1.1.4.3.3", erosaoContinental.Id, "Evolução do ravinamento, afetando também o escoamento subsuperficial (freático)."));

        // ---------------------------------------------------------------------------------
        // 1.2 HIDROLÓGICO
        // ---------------------------------------------------------------------------------
        var hidrologico = Create("Hidrológico", "1.2.0.0.0", naturais.Id);
        context.Naturezas.Add(hidrologico);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Inundações", "1.2.1.0.0", hidrologico.Id, "Submersão gradual de áreas fora dos limites normais de um curso de água (planícies)."));
        context.Naturezas.Add(Create("Enxurradas", "1.2.2.0.0", hidrologico.Id, "Escoamento superficial de alta velocidade e energia (Flash Floods)."));
        context.Naturezas.Add(Create("Alagamentos", "1.2.3.0.0", hidrologico.Id, "Acúmulo de água em áreas urbanas por deficiência de drenagem."));

        // ---------------------------------------------------------------------------------
        // 1.3 METEOROLÓGICO
        // ---------------------------------------------------------------------------------
        var meteorologico = Create("Meteorológico", "1.3.0.0.0", naturais.Id);
        context.Naturezas.Add(meteorologico);
        await context.SaveChangesAsync();

        // 1.3.1 Sistemas de Grande Escala
        var sistGrandeEscala = Create("Sistemas de grande escala/Escala regional", "1.3.1.0.0", meteorologico.Id);
        context.Naturezas.Add(sistGrandeEscala);
        await context.SaveChangesAsync();

        var ciclones = Create("Ciclones", "1.3.1.1.0", sistGrandeEscala.Id);
        context.Naturezas.Add(ciclones);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Ventos costeiros (mobilidade de dunas)", "1.3.1.1.1", ciclones.Id, "Movimentação de dunas de areia sobre construções na orla."));
        context.Naturezas.Add(Create("Marés de tempestade (ressaca)", "1.3.1.1.2", ciclones.Id, "Ondas violentas que geram maior agitação do mar e elevação do nível do oceano."));

        context.Naturezas.Add(Create("Frentes frias/Zonas de convergência", "1.3.1.2.0", sistGrandeEscala.Id, "Avanço de massa de ar frio ou zonas de baixa pressão provocando chuvas e ventos."));

        // 1.3.2 Tempestades
        var tempestades = Create("Tempestades", "1.3.2.0.0", meteorologico.Id);
        context.Naturezas.Add(tempestades);
        await context.SaveChangesAsync();

        var tempestadeLocal = Create("Tempestade local/Convectiva", "1.3.2.1.0", tempestades.Id);
        context.Naturezas.Add(tempestadeLocal);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Tornados", "1.3.2.1.1", tempestadeLocal.Id, "Coluna de ar giratória violenta em contato com a terra."));
        context.Naturezas.Add(Create("Tempestade de raios", "1.3.2.1.2", tempestadeLocal.Id, "Tempestade com intensa atividade elétrica."));
        context.Naturezas.Add(Create("Granizo", "1.3.2.1.3", tempestadeLocal.Id, "Precipitação de pedaços irregulares de gelo."));
        context.Naturezas.Add(Create("Chuvas intensas", "1.3.2.1.4", tempestadeLocal.Id, "Chuvas com acumulados significativos causando múltiplos desastres."));
        context.Naturezas.Add(Create("Vendaval", "1.3.2.1.5", tempestadeLocal.Id, "Forte deslocamento de uma massa de ar."));

        // 1.3.3 Temperaturas Extremas
        var tempExtremas = Create("Temperaturas extremas", "1.3.3.0.0", meteorologico.Id);
        context.Naturezas.Add(tempExtremas);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Onda de calor", "1.3.3.1.0", tempExtremas.Id, "Período prolongado excessivamente quente (mín 3 dias, 5°C acima da média)."));

        var ondaFrio = Create("Onda de frio", "1.3.3.2.0", tempExtremas.Id);
        context.Naturezas.Add(ondaFrio);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Friagem", "1.3.3.2.1", ondaFrio.Id, "Queda de temperatura mínima abaixo do esperado por 3 a 4 dias."));
        context.Naturezas.Add(Create("Geadas", "1.3.3.2.2", ondaFrio.Id, "Formação de camada de cristais de gelo na superfície."));

        // ---------------------------------------------------------------------------------
        // 1.4 CLIMATOLÓGICO
        // ---------------------------------------------------------------------------------
        var climatologico = Create("Climatológico", "1.4.0.0.0", naturais.Id);
        context.Naturezas.Add(climatologico);
        await context.SaveChangesAsync();

        // 1.4.1 Seca
        var secaGroup = Create("Seca", "1.4.1.0.0", climatologico.Id); // Note que o documento agrupa Estiagem/Seca/Incendio sob 1.4.1 no COBRADE
        context.Naturezas.Add(secaGroup);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Estiagem", "1.4.1.1.0", secaGroup.Id, "Período prolongado de baixa pluviosidade (perda de umidade > reposição)."));
        context.Naturezas.Add(Create("Seca", "1.4.1.2.0", secaGroup.Id, "Estiagem prolongada causando grave desequilíbrio hidrológico."));

        var incendio = Create("Incêndio florestal", "1.4.1.3.0", secaGroup.Id);
        context.Naturezas.Add(incendio);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Incêndios em áreas protegidas", "1.4.1.3.1", incendio.Id, "Fogo sem controle em parques e áreas de preservação."));
        context.Naturezas.Add(Create("Incêndios em áreas não protegidas", "1.4.1.3.2", incendio.Id, "Fogo sem controle em vegetação não protegida, afetando qualidade do ar."));

        context.Naturezas.Add(Create("Baixa umidade do ar", "1.4.1.4.0", secaGroup.Id, "Queda da taxa de vapor de água para abaixo de 20%."));

        // ---------------------------------------------------------------------------------
        // 1.5 BIOLÓGICO
        // ---------------------------------------------------------------------------------
        var biologico = Create("Biológico", "1.5.0.0.0", naturais.Id);
        context.Naturezas.Add(biologico);
        await context.SaveChangesAsync();

        // 1.5.1 Epidemias
        var epidemias = Create("Epidemias", "1.5.1.0.0", biologico.Id);
        context.Naturezas.Add(epidemias);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Doenças infecciosas virais", "1.5.1.1.0", epidemias.Id, "Aumento brusco de doenças geradas por vírus."));
        context.Naturezas.Add(Create("Doenças infecciosas bacterianas", "1.5.1.2.0", epidemias.Id, "Aumento brusco de doenças geradas por bactérias."));
        context.Naturezas.Add(Create("Doenças infecciosas parasíticas", "1.5.1.3.0", epidemias.Id, "Aumento brusco de doenças geradas por parasitas."));
        context.Naturezas.Add(Create("Doenças infecciosas fúngicas", "1.5.1.4.0", epidemias.Id, "Aumento brusco de doenças geradas por fungos."));

        // 1.5.2 Infestações/Pragas
        var pragas = Create("Infestações/Pragas", "1.5.2.0.0", biologico.Id);
        context.Naturezas.Add(pragas);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Infestações de animais", "1.5.2.1.0", pragas.Id, "Animais que alteram o equilíbrio ecológico."));

        var algas = Create("Infestações de algas", "1.5.2.2.0", pragas.Id);
        context.Naturezas.Add(algas);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Marés vermelhas", "1.5.2.2.1", algas.Id, "Aglomeração de microalgas mudando a cor da água."));
        context.Naturezas.Add(Create("Cianobactérias em reservatórios", "1.5.2.2.2", algas.Id, "Aglomeração em reservatórios receptores de dejetos."));

        context.Naturezas.Add(Create("Outras infestações", "1.5.2.3.0", pragas.Id, "Outras infestações que alterem o equilíbrio ecológico."));


        // =================================================================================
        // 2. TECNOLÓGICOS
        // =================================================================================
        var tecnologicos = Create("Tecnológicos", "2.0.0.0.0", null);
        context.Naturezas.Add(tecnologicos);
        await context.SaveChangesAsync();

        // ---------------------------------------------------------------------------------
        // 2.1 SUBSTÂNCIAS RADIOATIVAS
        // ---------------------------------------------------------------------------------
        var radioativos = Create("Desastres relacionados a substâncias radioativas", "2.1.0.0.0", tecnologicos.Id);
        context.Naturezas.Add(radioativos);
        await context.SaveChangesAsync();

        var siderais = Create("Desastres siderais com riscos radioativos", "2.1.1.0.0", radioativos.Id);
        context.Naturezas.Add(siderais);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Queda de satélite (radionuclídeos)", "2.1.1.1.0", siderais.Id, "Queda de satélites com material radioativo."));

        var equipRadioativos = Create("Desastres com substâncias e equipamentos radioativos", "2.1.2.0.0", radioativos.Id);
        context.Naturezas.Add(equipRadioativos);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Fontes radioativas em processos de produção", "2.1.2.1.0", equipRadioativos.Id, "Escapamento acidental de radiação em indústrias/pesquisas."));

        var poluicaoRad = Create("Riscos de intensa poluição ambiental por resíduos radioativos", "2.1.3.0.0", radioativos.Id);
        context.Naturezas.Add(poluicaoRad);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Outras fontes de liberação de radionuclídeos", "2.1.3.1.0", poluicaoRad.Id, "Escapamento de radiação de fontes diversas."));

        // ---------------------------------------------------------------------------------
        // 2.2 PRODUTOS PERIGOSOS
        // ---------------------------------------------------------------------------------
        var prodPerigosos = Create("Desastres relacionados a produtos perigosos", "2.2.0.0.0", tecnologicos.Id);
        context.Naturezas.Add(prodPerigosos);
        await context.SaveChangesAsync();

        var industriasPP = Create("Desastres em plantas/distritos industriais com produtos perigosos", "2.2.1.0.0", prodPerigosos.Id);
        context.Naturezas.Add(industriasPP);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Liberação de produtos químicos para atmosfera", "2.2.1.1.0", industriasPP.Id, "Explosão ou incêndio liberando químicos."));

        var contaminacaoAgua = Create("Desastres relacionados à contaminação da água", "2.2.2.0.0", prodPerigosos.Id);
        context.Naturezas.Add(contaminacaoAgua);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Liberação de químicos em água potável", "2.2.2.1.0", contaminacaoAgua.Id, "Derramamento em sistema de abastecimento."));
        context.Naturezas.Add(Create("Derramamento em ambiente lacustre/fluvial/marinho", "2.2.2.2.0", contaminacaoAgua.Id, "Derramamento em rios, lagos ou mar."));

        var conflitosBelicos = Create("Desastres relacionados a conflitos bélicos", "2.2.3.0.0", prodPerigosos.Id);
        context.Naturezas.Add(conflitosBelicos);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Ações militares (químico/biológico/radiológico)", "2.2.3.1.0", conflitosBelicos.Id, "Agentes perigosos usados em atentados ou guerra."));

        var transportePP = Create("Transporte de produtos perigosos", "2.2.4.0.0", prodPerigosos.Id);
        context.Naturezas.Add(transportePP);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Transporte rodoviário", "2.2.4.1.0", transportePP.Id, "Extravasamento no modal rodoviário."));
        context.Naturezas.Add(Create("Transporte ferroviário", "2.2.4.2.0", transportePP.Id, "Extravasamento no modal ferroviário."));
        context.Naturezas.Add(Create("Transporte aéreo", "2.2.4.3.0", transportePP.Id, "Extravasamento no modal aéreo."));
        context.Naturezas.Add(Create("Transporte dutoviário", "2.2.4.4.0", transportePP.Id, "Extravasamento no modal dutoviário."));
        context.Naturezas.Add(Create("Transporte marítimo", "2.2.4.5.0", transportePP.Id, "Extravasamento no modal marítimo."));
        context.Naturezas.Add(Create("Transporte aquaviário", "2.2.4.6.0", transportePP.Id, "Extravasamento no modal aquaviário."));


        // ---------------------------------------------------------------------------------
        // 2.3 INCÊNDIOS URBANOS
        // ---------------------------------------------------------------------------------
        var incendiosUrbanos = Create("Desastres relacionados a incêndios urbanos", "2.3.0.0.0", tecnologicos.Id);
        context.Naturezas.Add(incendiosUrbanos);
        await context.SaveChangesAsync();

        var incendiosUrbTipos = Create("Incêndios urbanos", "2.3.1.0.0", incendiosUrbanos.Id); // Subgrupo redundante no nome, mas existe na hierarquia
        context.Naturezas.Add(incendiosUrbTipos);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Incêndios em plantas industriais/depósitos", "2.3.1.1.0", incendiosUrbTipos.Id, "Propagação descontrolada do fogo em indústrias/depósitos."));
        context.Naturezas.Add(Create("Incêndios em aglomerados residenciais", "2.3.1.2.0", incendiosUrbTipos.Id, "Propagação descontrolada do fogo em conjuntos habitacionais."));

        // ---------------------------------------------------------------------------------
        // 2.4 OBRAS CIVIS
        // ---------------------------------------------------------------------------------
        var obrasCivis = Create("Desastres relacionados a obras civis", "2.4.0.0.0", tecnologicos.Id);
        context.Naturezas.Add(obrasCivis);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Colapso de edificações", "2.4.1.0.0", obrasCivis.Id, "Queda de estrutura civil."));
        context.Naturezas.Add(Create("Rompimento/colapso de barragens", "2.4.2.0.0", obrasCivis.Id, "Rompimento ou colapso de barragens."));

        // ---------------------------------------------------------------------------------
        // 2.5 TRANSPORTE DE PASSAGEIROS E CARGAS NÃO PERIGOSAS
        // ---------------------------------------------------------------------------------
        var transporteNaoPerigoso = Create("Transporte de passageiros e cargas não perigosas", "2.5.0.0.0", tecnologicos.Id);
        context.Naturezas.Add(transporteNaoPerigoso);
        await context.SaveChangesAsync();
        context.Naturezas.Add(Create("Transporte rodoviário", "2.5.1.0.0", transporteNaoPerigoso.Id, "Acidente rodoviário (passageiros/cargas comuns)."));
        context.Naturezas.Add(Create("Transporte ferroviário", "2.5.2.0.0", transporteNaoPerigoso.Id, "Acidente ferroviário (passageiros/cargas comuns)."));
        context.Naturezas.Add(Create("Transporte aéreo", "2.5.3.0.0", transporteNaoPerigoso.Id, "Acidente aéreo (passageiros/cargas comuns)."));
        context.Naturezas.Add(Create("Transporte marítimo", "2.5.4.0.0", transporteNaoPerigoso.Id, "Acidente marítimo (passageiros/cargas comuns)."));
        context.Naturezas.Add(Create("Transporte aquaviário", "2.5.5.0.0", transporteNaoPerigoso.Id, "Acidente aquaviário (passageiros/cargas comuns)."));

        await context.SaveChangesAsync();
    }
}