using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.TokenGenerator;

namespace SIG_DefesaCivil.API.Services
{
    public class AuthService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly TokenService _tokenService;
        private readonly UserManager<Usuario> _userManager;

        public AuthService(
            DefesaCivilDbContext context,
            TokenService tokenService,
            UserManager<Usuario> userManager)
        {
            _context = context;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<AuthResponseDTO> RenovarTokenAsync(string tokenAtual)
        {
            // 1. Busca o token no banco
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == tokenAtual);

            // 2. Validações
            if (storedToken == null)
                throw new Exception("Token inexistente.");

            if (!storedToken.IsActive)
                throw new Exception("Token expirado ou revogado.");

            // 3. Revoga o token antigo (Rotação de Token)
            storedToken.Revoked = DateTime.UtcNow;
            _context.RefreshTokens.Update(storedToken);

            // 4. Busca o usuário dono do token
            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
                throw new Exception("Usuário não encontrado.");

            // 5. Gera novos tokens
            var newAccessToken = await _tokenService.GenerateJwtTokenAsync(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

            // 6. Salva o novo refresh token no banco
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            // 7. Retorna o DTO
            return new AuthResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Senha))
                throw new Exception("Usuário ou senha inválidos.");

            // Gera Tokens
            var accessToken = await _tokenService.GenerateJwtTokenAsync(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            // Salva Refresh Token no Banco
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                PrimeiroAcesso = user.isPrimeiroAcesso,
                Message = "Login realizado com sucesso."
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return;

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (storedToken != null)
            {
                storedToken.Revoked = DateTime.UtcNow;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("Usuário não encontrado.");

            return await _userManager.GetRolesAsync(user);
        }

        public async Task AlterarSenhaAsync(string userId, string novaSenha)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("Usuário não encontrado");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, novaSenha);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            user.isPrimeiroAcesso = false;
            await _userManager.UpdateAsync(user);
        }

    }
}