using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.TokenGenerator;

namespace SIG_DefesaCivil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly  DefesaCivilDbContext _context;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AccountsController(UserManager<Usuario> userManager,RoleManager<IdentityRole> roleManager , DefesaCivilDbContext context, JwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [AllowAnonymous] //todo: permitir que apenas administradores acessem este end-point.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO Register)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emailEmUso = await _userManager.FindByEmailAsync(Register.Email);
            if (emailEmUso != null)
                return BadRequest("E-mail já está em uso.");

            var user = new Usuario
            {
                UserName = Register.Email,
                Email = Register.Email,
                Nome = Register.Nome,
                Telefone = Register.Telefone,
                CPF = Register.CPF,
                DataAdmissao = Register.DataAdmissao,
                isAtivo = Register.IsAtivo
            };

            var result = await _userManager.CreateAsync(user, Register.Senha);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var rolesPermitidas = new[] { "Administrador", "Diretor", "Usuário de campo" };
            if (!rolesPermitidas.Contains(Register.Cargo))
                return BadRequest("Cargo inválido.");

            if (!await _roleManager.RoleExistsAsync(Register.Cargo))
                await _roleManager.CreateAsync(new IdentityRole(Register.Cargo));

            await _userManager.AddToRoleAsync(user, Register.Cargo);

            return Ok(new { message = "Usuário registrado com sucesso." });
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

            return Ok(new { message = "Login realizado com sucesso." });
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

    }
}
