using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApi.DAL.DataContext;
using WebApi.DAL.Repository.Interfaces;
using WebApi.Entity.DTOs;
using WebApi.Entity.Models;

namespace WebApi.DAL.Repository.Implementacion
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly AplicationDBContext _context;
        private readonly ILogger<UsuarioRepositorio> _logger;

        public UsuarioRepositorio(AplicationDBContext context, ILogger<UsuarioRepositorio> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Usuario> ObtenerPorEmailYPaswordAsync(string email, string password)
        {
            return await _context.Usuario.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Correo == email && u.Clave == password);
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _context.Usuario.AsNoTracking().AnyAsync(u => u.Correo == email);
        }

        public async Task<bool> GuardarUsuarioAsync(Usuario usuario)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                usuario.FechaCreacion = DateTime.UtcNow;
                await _context.Usuario.AddAsync(usuario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al guardar el usuario: {Usuario}", usuario.Nombre);
                return false;
            }
        }

        private async Task<(bool, string)> ValidarUsuarioDtoAsync(UsuarioDTO usuarioDto, int? id = null)
        {
            if (!string.IsNullOrEmpty(usuarioDto.Email))
            {
                bool emailExistente = await _context.Usuario.AnyAsync(u => u.Correo == usuarioDto.Email && (id == null || u.IdUsuario != id));
                if (emailExistente)
                    return (false, "Esta dirección de correo electrónico ya está en uso");
            }

            // Puedes añadir más validaciones comunes aquí si es necesario

            return (true, string.Empty);
        }

        public async Task<bool> UsuarioExisteAsync(int id)
        {
            return await _context.Usuario.AsNoTracking().AnyAsync(u => u.IdUsuario == id);
        }

                       
        public async Task<bool> GuardarCambiosAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar los cambios en la base de datos");
                return false;
            }
        }
    }
}
