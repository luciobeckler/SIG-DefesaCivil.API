using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.TokenGenerator;

namespace SIG_DefesaCivil.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
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
                return BadRequest("E-mail já está em uso.");

            if (!await _roleManager.RoleExistsAsync(dto.Permissao))
                return BadRequest("Permissão inválida.");

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

            var result = await _userManager.CreateAsync(user, dto.Senha);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, dto.Permissao);

            return Ok(new { message = $"Usuário {user.Nome} registrado com sucesso."});
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            var result = await _userManager.DeleteAsync(usuario);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Usuário deletado com sucesso." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RegisterDTO updatedUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            usuario.Nome = updatedUser.Nome;
            usuario.Email = updatedUser.Email;
            usuario.UserName = updatedUser.Email;
            usuario.Telefone = updatedUser.Telefone;
            usuario.CPF = updatedUser.CPF;
            usuario.Cargo = updatedUser.Cargo;
            usuario.DataAdmissao = updatedUser.DataAdmissao;
            usuario.isAtivo = updatedUser.IsAtivo;
            usuario.Endereco = updatedUser.Endereco;
            usuario.DataDeNascimento = updatedUser.DataDeNascimento;

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Atualizar a senha, se fornecida
            if (!string.IsNullOrEmpty(updatedUser.Senha))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var passResult = await _userManager.ResetPasswordAsync(usuario, token, updatedUser.Senha);
                if (!passResult.Succeeded)
                    return BadRequest(passResult.Errors);
            }

            // Atualizar cargo/role
            var currentRoles = await _userManager.GetRolesAsync(usuario);
            if (currentRoles.FirstOrDefault() != updatedUser.Permissao)
            {
                await _userManager.RemoveFromRolesAsync(usuario, currentRoles);

                if (!await _roleManager.RoleExistsAsync(updatedUser.Permissao))
                    await _roleManager.CreateAsync(new IdentityRole(updatedUser.Permissao));

                await _userManager.AddToRoleAsync(usuario, updatedUser.Permissao);
            }

            return Ok(new { message = "Usuário atualizado com sucesso." });
        }

        [HttpGet("roles")]
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
