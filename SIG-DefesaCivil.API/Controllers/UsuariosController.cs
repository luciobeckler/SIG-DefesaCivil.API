using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.TokenGenerator;
using System.Security.Claims;

namespace SIG_DefesaCivil.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DefesaCivilDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public UsuariosController(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, DefesaCivilDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        //todo: permitir que apenas administradores acessem este end-point.

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emailEmUso = await _userManager.FindByEmailAsync(dto.Email);
            if (emailEmUso != null)
                return BadRequest(new { message = "E-mail já está em uso." });

            var cpfEmUso = await _userManager.Users.AnyAsync(u => u.CPF == dto.CPF);
            if (cpfEmUso)
                return BadRequest(new { message = "CPF já está em uso." });

            var telefoneEmUso = await _userManager.Users.AnyAsync(u => u.Telefone == dto.Telefone);
            if (telefoneEmUso)
                return BadRequest(new { message = "Telefone já está em uso" });

            if (!await _roleManager.RoleExistsAsync(dto.Permissao))
                return BadRequest(new { message = "Permissão inválida." });

            var user = new Usuario
            {
                UserName = dto.Email,
                Email = dto.Email,
                Nome = dto.Nome,
                Telefone = dto.Telefone,
                CPF = dto.CPF,
                DataAdmissao = dto.DataAdmissao,
                Cargo = dto.Cargo,
                Endereco = dto.Endereco,
                DataDeNascimento = dto.DataDeNascimento,
                isAtivo = dto.IsAtivo
            };

            var result = await _userManager.CreateAsync(user, "SenhaPadrao123*");

            if (!result.Succeeded)
                return BadRequest(new { message = result.Errors });

            await _userManager.AddToRoleAsync(user, dto.Permissao);

            return Ok(new
            {
                message = $"Usuário {user.Nome} registrado com sucesso. A senha padrão dele é \"SenhaPadrao123*\", lembre o usuário de trocar a senha no primeiro acesso."
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _userManager.Users.ToListAsync();

            var retorno = usuarios
                .Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Email,
                    u.Telefone,
                    u.CPF,
                    u.DataAdmissao,
                    u.isAtivo,
                    u.Cargo,
                    Permissao = _userManager.GetRolesAsync(u).Result.FirstOrDefault(),
                    u.Endereco,
                    u.DataDeNascimento
                });

            return Ok(retorno);
        }

        [HttpGet("outros-usuarios")]
        public async Task<IActionResult> GetAllOtherUsers()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var usuarios = await _userManager
                .Users
                .Where(u => u.Id != userId)
                .ToListAsync();

            var retorno = usuarios
                .Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Email,
                    u.Telefone,
                    u.CPF,
                    u.DataAdmissao,
                    u.isAtivo,
                    u.Cargo,
                    Permissao = _userManager.GetRolesAsync(u).Result.FirstOrDefault(),
                    u.Endereco,
                    u.DataDeNascimento
                });

            return Ok(retorno);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuário não encontrado." });

            var result = await _userManager.DeleteAsync(usuario);
            if (!result.Succeeded)
                return BadRequest(new { message = result.Errors });

            return Ok(new { message = "Usuário deletado com sucesso." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState });

            var emailEmUso = await _userManager.Users.AnyAsync(u => (u.Email == dto.Email) && (u.Id != id));
            if (emailEmUso)
                return BadRequest(new { message = "E-mail já está em uso." });

            var cpfEmUso = await _userManager.Users.AnyAsync(u => (u.CPF == dto.CPF) && (u.Id != id) );
            if (cpfEmUso)
                return BadRequest(new { message = "CPF já está em uso." });

            var telefoneEmUso = await _userManager.Users.AnyAsync(u => (u.Telefone == dto.Telefone) && (u.Id != id));
            if (telefoneEmUso)
                return BadRequest(new { message = "Telefone já está em uso" });

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuário não encontrado." });

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.UserName = dto.Email;
            usuario.Telefone = dto.Telefone;
            usuario.CPF = dto.CPF;
            usuario.Cargo = dto.Cargo;
            usuario.DataAdmissao = dto.DataAdmissao;
            usuario.isAtivo = dto.IsAtivo;
            usuario.Endereco = dto.Endereco;
            usuario.DataDeNascimento = dto.DataDeNascimento;

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
                return BadRequest(new { message = result.Errors });

            // Atualizar cargo/role
            var currentRoles = await _userManager.GetRolesAsync(usuario);
            if (currentRoles.FirstOrDefault() != dto.Permissao)
            {
                await _userManager.RemoveFromRolesAsync(usuario, currentRoles);

                if (!await _roleManager.RoleExistsAsync(dto.Permissao))
                    await _roleManager.CreateAsync(new IdentityRole(dto.Permissao));

                await _userManager.AddToRoleAsync(usuario, dto.Permissao);
            }

            return Ok(new { message = "Usuário atualizado com sucesso." });
        }

        [HttpGet("get-all-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles
                .Select(r => new
                {
                    r.Id,
                    r.Name
                })
                .ToListAsync();

            return Ok(roles);
        }

    }
}
