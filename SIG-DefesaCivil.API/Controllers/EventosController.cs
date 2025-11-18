using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.DTO.Eventos;
using SIG_DefesaCivil.API.DTO.Eventos.SIG_DefesaCivil.API.DTO.EventoDTO;
using SIG_DefesaCivil.API.Enums;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;
using SIG_DefesaCivil.API.Services;
using System.Security.Claims;

namespace SIG_DefesaCivil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventoController : ControllerBase
    {
        private readonly EventoService _service;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        public EventoController(EventoService service, UserManager<Usuario> userManager, IMapper mapper)
        {
            _service = service;
            _userManager = userManager;
            _mapper = mapper;
        }
        
    }
}