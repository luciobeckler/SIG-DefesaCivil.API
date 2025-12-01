using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Identity;
using SIG_DefesaCivil.API.Models;
using System.Security.Claims;

namespace SIG_DefesaCivil.API.Services
{
    public class UsuarioService
    {
        private readonly UserManager<Usuario> _userManager;
        public UsuarioService(UserManager<Usuario> userManager) 
        {
            _userManager = userManager;
        }

        public async Task<Usuario> GetUsuarioAtual(ClaimsPrincipal User)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Usuário não encontrado no token.");
            }
            return await _userManager.FindByIdAsync(userId);
        }
    }
}
