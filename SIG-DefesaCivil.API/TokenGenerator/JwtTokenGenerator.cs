using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SIG_DefesaCivil.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SIG_DefesaCivil.API.TokenGenerator
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _config;
        private readonly UserManager<Usuario> _userManager;

        public JwtTokenGenerator(IConfiguration config, UserManager<Usuario> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> GenerateToken(Usuario user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
