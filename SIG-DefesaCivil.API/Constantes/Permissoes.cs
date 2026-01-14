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
}