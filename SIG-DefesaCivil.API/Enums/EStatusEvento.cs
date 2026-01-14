namespace SIG_DefesaCivil.API.Enums
{
    public enum EStatusEvento
    {
        /// <summary>
        /// O ocorrencia foi registrado no sistema, mas aguarda avaliação.
        /// </summary>
        Pendente,

        /// <summary>
        /// O ocorrencia está sendo avaliado por uma equipe para determinar a necessidade de ação.
        /// </summary>
        EmAnalise,

        /// <summary>
        /// Equipes foram despachadas e estão atuando no ocorrencia.
        /// </summary>
        EmAtendimento,

        /// <summary>
        /// A fase crítica do atendimento foi concluída, mas a situação ainda requer observação.
        /// </summary>
        EmMonitoramento,

        /// <summary>
        /// O ocorrencia foi completamente resolvido e todas as ações foram concluídas.
        /// </summary>
        Finalizado,

        /// <summary>
        /// O ocorrencia foi cancelado antes do início do atendimento (ex: alarme falso).
        /// </summary>
        Cancelado
    }
}