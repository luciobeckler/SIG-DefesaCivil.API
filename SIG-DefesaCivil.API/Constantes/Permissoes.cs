namespace SIG_DefesaCivil.API.Constants
{
    public static class Permissoes
    {
        private const string RoleAdministrador = nameof(Enums.ECargos.Administrador);
        private const string RoleDiretor = nameof(Enums.ECargos.Diretor);
        private const string RoleAgente = nameof(Enums.ECargos.AgenteDeCampo);

        public const string ApenasAdmin = RoleAdministrador;
        public const string DiretorOuSuperior = RoleDiretor + "," + RoleAdministrador;
        public const string AgenteOuSuperior = RoleAgente + "," + RoleDiretor + "," + RoleAdministrador;
    }

    public static class Permissions
    {
        // Ocorrências
        public const string OcorrenciaCriar = "ocorrencia:criar";
        public const string OcorrenciaEditar = "ocorrencia:editar";
        public const string OcorrenciaExcluir = "ocorrencia:excluir";
        public const string OcorrenciaVisualizarTodas = "ocorrencia:visualizar todas";
        public const string OcorrenciaVisualizarHistorico = "ocorrencia:visualizar histórico";

        // Gestão
        public const string UsuariosGerenciar = "usuarios:gerenciar";
        public const string NaturezasGerenciar = "naturezas:gerenciar";
    }

    public static class RolePermissions
    {
        // Dicionário: [Nome da Role] -> [Lista de Permissões]
        private static readonly Dictionary<string, List<string>> _map = new()
        {
            { "Administrador", new List<string> {
                Permissions.OcorrenciaCriar,
                Permissions.OcorrenciaEditar,
                Permissions.OcorrenciaExcluir,
                Permissions.OcorrenciaVisualizarTodas,
                Permissions.OcorrenciaVisualizarHistorico,
                Permissions.NaturezasGerenciar,
                Permissions.UsuariosGerenciar
            }},
            { "Diretor", new List<string> {
                Permissions.OcorrenciaCriar,
                Permissions.OcorrenciaEditar,
                Permissions.OcorrenciaExcluir,
                Permissions.OcorrenciaVisualizarTodas,
                Permissions.OcorrenciaVisualizarHistorico,
                Permissions.NaturezasGerenciar,
                Permissions.UsuariosGerenciar
            }},
            { "AgenteDeCampo", new List<string> {
                Permissions.OcorrenciaCriar,
            }}
        };

        public static List<string> GetByRole(string role)
        {
            return _map.ContainsKey(role) ? _map[role] : new List<string>();
        }

        public static IEnumerable<string> GetAllRoles()
        {
            return _map.Keys;
        }
    }
}
