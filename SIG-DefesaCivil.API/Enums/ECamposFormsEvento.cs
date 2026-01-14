using System.Text.Json.Serialization;

namespace SIG_DefesaCivil.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EAnalisePreliminar { Sim, Nao, Vistoria, Orientacao, Arquivamento, Outros }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ECaracterizacaoLocal { Encosta, FundoDeVale, CorregoRio, Aterro, DeCorte }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ETipoEdificacao { Barracao, Casa, Predio, Outros }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ETipoEstrutura { Alvenaria, Madeira, ConcretoArmado, PreFabricado, OutrosMateriais }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ETipoRisco { Construtivo, Geologico, Biologico, Outros }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EGrauRisco { MuitoAlto, Alto, Medio, Baixo } // Geralmente Single Select

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ERegimeOcupacao { Proprio, Alugado, Outros } // Geralmente Single Select

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ETipificacaoOcorrencia
    {
        Escorregamento, Trincas, DegrauDeAbatimento, InundacaoDeCorrego,
        Incendio, Solapamento, CicatrizDeEscorregamento, Erosao,
        Alagamento, AbatimentoDeFossa, RedePublicaDeDrenagemPluvialRompida,
        RolamentoTombamentoDeBlocos
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EMotivacao
    {
        Rachaduras, Infiltracao, MovimentacaoDeSolo, Arvore,
        DesabamentoTotal, DesabamentoParcial, PrecarioInsalubre, Encosta,
        LancamentoDeAguaPluvialEsgoto, LancamentoDeLixoEntulhoAterro,
        DesprendimentoDeReboco, InexistenciaInsuficienciaDeDrenagemPluvial, Outros
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EAreaAfetada { Residencia, Muro, Ponte }
}
