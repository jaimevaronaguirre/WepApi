using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApi.Entity.Models;
using WebApi.DAL.Repository.Interfaces;
using WebApi.DAL.DataContext;

namespace WebApi.DAL.Repository.Implementacion
{
    public class ProductoRepositorio : IProductoRepositorio
    {
        private readonly DbContext _context;
        private readonly ILogger<ProductoRepositorio> _logger;


        public ProductoRepositorio(AplicationDBContext context, ILogger<ProductoRepositorio> logger)
        {
            _context = context;
            _logger = logger;
        }       

        public async Task<List<Producto>> ObtenerTodos()
        {
            try
            {
                return await _context.Set<Producto>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los productos");
                return new List<Producto>();
            }
        }

        public async Task<Producto> ObtenerProductoPorId(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando búsqueda de producto con ID {Id}", id);
                var producto = await _context.Set<Producto>()
                    .Where(p => p.IdProducto == id)
                    .FirstOrDefaultAsync();

                if (producto == null)
                {
                    _logger.LogWarning("Producto con ID {Id} no encontrado", id);
                    return null;
                }

                _logger.LogInformation("Producto con ID {Id} encontrado", id);
                return producto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {Id}", id);
                throw;
            }
        }

        // Método para obtener el producto por nombre y marca
        public async Task<Producto> ObtenerProductoPorNombreYMarca(string nombre, string marca)
        {
            try
            {
                var producto = await _context.Set<Producto>()
                    .FirstOrDefaultAsync(p => p.Nombre == nombre && p.Marca == marca);
                return producto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto por nombre y marca");
                throw;
            }
        }
        public async Task<bool> ProductoExisteAsync(int id)
        {
            try
            {
                return await _context.Set<Producto>().AnyAsync(p => p.IdProducto == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si el producto existe");
                return false;
            }
        }

        public async Task<bool> GuardarProductoAsync(Producto producto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Set<Producto>().AddAsync(producto);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el producto");
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> ActualizarProductoAsync(int id, Producto productoParcial)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var productoExistente = await _context.Set<Producto>().FindAsync(id);
                    if (productoExistente == null)
                    {
                        return false;
                    }

                    _context.Entry(productoExistente).CurrentValues.SetValues(productoParcial);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error al actualizar el producto");
                    return false;
                }
            }
        }

        public async Task<bool> EliminarProductoAsync(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var producto = await _context.Set<Producto>().FindAsync(id);
                    if (producto == null)
                    {
                        return false;
                    }

                    _context.Set<Producto>().Remove(producto);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error al eliminar el producto");
                    return false;
                }
            }
        }


        public async Task<bool> GuardarCambiosAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar los cambios");
                return false;
            }
        }
        
       
    }
}

