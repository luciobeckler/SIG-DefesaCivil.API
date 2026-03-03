using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SIG_DefesaCivil.API.Data.Constantes;
using SIG_DefesaCivil.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SIG_DefesaCivil.API.Services.TokenGenerator
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<Usuario> _userManager;

        public TokenService(IConfiguration config, UserManager<Usuario> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> GenerateJwtTokenAsync(Usuario user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            // Se o usuário não tiver role, defina um padrão ou lance erro
            var roleName = userRoles.FirstOrDefault() ?? "AgenteDeCampo";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, roleName), // Usa a role buscada
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // Pega as permissões baseadas na Role encontrada
            var permissoesDoUsuario = RolePermissions.GetByRole(roleName);

            foreach (var perm in permissoesDoUsuario)
            {
                claims.Add(new Claim("Permissions", perm));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(string userId)
        {
            var randomNumber = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };
        }
    }
}
