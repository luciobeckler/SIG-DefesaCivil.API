using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.TokenGenerator;
using System.Security.Claims;

namespace SIG_DefesaCivil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsController(
            UserManager<Usuario> userManager,
            RoleManager<IdentityRole> roleManager, 
            JwtTokenGenerator jwtTokenGenerator
        ){
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Senha))
                return Unauthorized("Usuário ou senha inválidos");

            var token = await _jwtTokenGenerator.GenerateToken(user);

            Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(2),
                Path = "/"
            });

            return Ok(new
            {
                message = "Login realizado com sucesso.",
                primeiroAcesso = user.isPrimeiroAcesso
            });
        }

        [Authorize]
        [HttpPost("alterar-senha")]
        public async Task<IActionResult> AlterarSenha([FromBody] AccountDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("Usuário não encontrado");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NovaSenha);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            user.isPrimeiroAcesso = false;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Senha alterada com sucesso" });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Remove o cookie do token JWT
            Response.Cookies.Append("auth_token", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1),
                Path = "/"
            });

            return Ok(new { message = "Logout realizado com sucesso." });
        }

        [Authorize]
        [HttpGet("get-account-roles")]
        public async Task<IActionResult> GetAccountRoles()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }

    }
}
