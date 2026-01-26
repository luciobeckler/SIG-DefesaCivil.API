using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.Constants;

namespace SIG_DefesaCivil.API.Controllers
{
    [ApiController]
    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        [HttpGet("permissions")]
        [AllowAnonymous] // Deve ser público para o app carregar ao abrir
        public IActionResult GetPermissionsMap()
        {
            // Usa Reflection para transformar a classe estática Permissions em um JSON
            // Retorno: { "OcorrenciaCriar": "ocorrencia:criar", ... }
            var permissions = typeof(Permissions)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .ToDictionary(fi => fi.Name, fi => fi.GetRawConstantValue().ToString());

            return Ok(permissions);
        }
    }
}
