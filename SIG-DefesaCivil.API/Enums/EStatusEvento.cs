namespace SIG_DefesaCivil.API.Enums
{
    public enum EStatusEvento
    {
        /// <summary>
        /// O evento foi registrado no sistema, mas aguarda avaliação.
        /// </summary>
        Pendente,

        /// <summary>
        /// O evento está sendo avaliado por uma equipe para determinar a necessidade de ação.
        /// </summary>
        EmAnalise,

        /// <summary>
        /// Equipes foram despachadas e estão atuando no evento.
        /// </summary>
        EmAtendimento,

        /// <summary>
        /// A fase crítica do atendimento foi concluída, mas a situação ainda requer observação.
        /// </summary>
        EmMonitoramento,

        /// <summary>
        /// O evento foi completamente resolvido e todas as ações foram concluídas.
        /// </summary>
        Finalizado,

        /// <summary>
        /// O evento foi cancelado antes do início do atendimento (ex: alarme falso).
        /// </summary>
        Cancelado
    }
}