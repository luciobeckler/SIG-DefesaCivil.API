using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Mappers
{
    public static class UsuarioMapper
    {
        public static ResponsavelDTO ToDto(this Usuario entity)
        {
            if (entity == null) return null!;

            return new ResponsavelDTO
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Email = entity.Email
            };
        }
    }
}
