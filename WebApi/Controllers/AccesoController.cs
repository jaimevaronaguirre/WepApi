using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.BLL.Repository.Interfaces;
using WebApi.Entity.DTOs;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]    
    [ApiController]

    public class AccesoController : ControllerBase
    {
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IMapper _mapper;
        private readonly ILogger<AccesoController> _logger;

        public AccesoController(IUsuarioServicio usuarioServicio, IMapper mapper, ILogger<AccesoController> logger)
        {
            _usuarioServicio = usuarioServicio;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpPost("Registrase")]
        [AllowAnonymous] // Permitir acceso anónimo
        [ProducesResponseType(StatusCodes.Status201Created)] // Creación exitosa
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Datos inválidos        
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Conflicto (usuario ya existente)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Error del servidor
        public async Task<IActionResult> Registrarse([FromBody] UsuarioDTO usuarioDTO)
        {
            var (exito, mensaje) = await _usuarioServicio.RegistrarUsuarioAsync(usuarioDTO);
            if (!exito)
            {
                return BadRequest(new { Mensaje = mensaje });// Devuelve un código HTTP 400 y el mensaje
            }
            return Ok(new { Mensaje = mensaje });
        }

        [HttpPost("Login")]
        [AllowAnonymous] // Permitir acceso anónimo
        [ProducesResponseType(StatusCodes.Status200OK)] // Solicitud exitosa
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Usuario no encontrado
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Error interno del servidor
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var (exito, mensaje, token) = await _usuarioServicio.LoginAsync(loginDTO);

            if (!exito)
            {
                return Unauthorized(new { Mensaje = mensaje }); // Codigo HTTP 401
            }

            return Ok(new { Mensaje = mensaje, Token = token }); // Codigo HTTP 200 Ok
        }

        private bool UserHasPermission(int userId)
        {
            // Implementa la lógica para verificar si el usuario tiene permisos
            return true; // Ajusta esta línea según la lógica de tu aplicación
        }

    }
}
