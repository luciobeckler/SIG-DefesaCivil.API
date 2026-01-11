using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Services;
using System.Security.Claims;

namespace SIG_DefesaCivil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDto);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("alterar-senha")]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaDTO dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                await _authService.AlterarSenhaAsync(userId, dto.NovaSenha);
                return Ok(new { message = "Senha alterada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDTO dto)
        {
            await _authService.LogoutAsync(dto.RefreshToken);

            return Ok(new { message = "Logout realizado com sucesso." });
        }

        [Authorize]
        [HttpGet("get-account-roles")]
        public async Task<IActionResult> GetAccountRoles()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var roles = await _authService.GetUserRolesAsync(userId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDTO>> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                    return BadRequest("Token inválido.");

                var resultado = await _authService.RenovarTokenAsync(request.RefreshToken);

                return Ok(resultado);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }
}