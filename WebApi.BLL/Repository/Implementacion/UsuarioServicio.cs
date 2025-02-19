using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApi.BLL.Repository.Interfaces;
using WebApi.BLL.Utilidades;
using WebApi.DAL.Repository.Interfaces;
using WebApi.Entity.DTOs;
using WebApi.Entity.Models;

namespace WebApi.BLL.Repository.Implementacion
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly EncriptadorSHA256 _encriptadorSHA256;
        private readonly GeneradorJWT _generadorJWT;
        private readonly ILogger<UsuarioServicio> _logger;

        public UsuarioServicio(
            IUsuarioRepositorio usuarioRepositorio,
            EncriptadorSHA256 encriptadorSHA256,
            GeneradorJWT generadorJWT,
            ILogger<UsuarioServicio> logger)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _encriptadorSHA256 = encriptadorSHA256;
            _generadorJWT = generadorJWT;
            _logger = logger;
        }

        public async Task<(bool, string)> RegistrarUsuarioAsync(UsuarioDTO usuarioDto)
        {
            try
            {
                _logger.LogInformation("Intentando registrar usuario con email: {Email}", usuarioDto.Email);

                // Verificar si ya existe un usuario registrado
                var usuarioExistente = await _usuarioRepositorio.EmailExisteAsync(usuarioDto.Email);
                if (usuarioExistente)
                {
                    _logger.LogWarning("El email {Email} ya está en uso.", usuarioDto.Email);
                    return (false, "Esta dirección de correo electrónico ya está en uso.");
                }

                // Crear usuario
                var usuario = new Usuario
                {
                    Nombre = usuarioDto.Nombre,
                    Correo = usuarioDto.Email,
                    Clave = _encriptadorSHA256.encriptarSHA256(usuarioDto.Clave),
                };

                _logger.LogInformation("Guardando usuario con email: {Email}", usuarioDto.Email);
                var resultado = await _usuarioRepositorio.GuardarUsuarioAsync(usuario);
                if (!resultado)
                {
                    _logger.LogWarning("No se pudo guardar el usuario con email: {Email}", usuarioDto.Email);
                    return (false, "No se pudo guardar el usuario.");
                }

                await _usuarioRepositorio.GuardarCambiosAsync();
                _logger.LogInformation("Usuario registrado exitosamente con ID: {IdUsuario}", usuario.IdUsuario);

                return (true, "Usuario registrado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en RegistrarUsuarioAsync");
                return (false, $"Error inesperado: {ex.Message}");
            }
        }


        public async Task<(bool Exito, string Mensaje, string Token)> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                _logger.LogInformation("Intentando login para el email: {Email}", loginDto.Email);

                var passwordEncriptado = _encriptadorSHA256.encriptarSHA256(loginDto.Clave);
                var usuario = await _usuarioRepositorio.ObtenerPorEmailYPaswordAsync(loginDto.Email, passwordEncriptado);

                if (usuario == null)
                {
                    _logger.LogWarning("Intento de login fallido para el email: {Email}", loginDto.Email);
                    return (false, "Usuario no encontrado", null);
                }

                var token = _generadorJWT.generarJWT(usuario);

                _logger.LogInformation("Login exitoso para el email: {Email}", loginDto.Email);
                return (true, "Login exitoso", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en LoginAsync");
                return (false, $"Error inesperado: {ex.Message}", null);
            }
        }
    }
}
