using Microsoft.Extensions.Logging;
using WebApi.BLL.Repository.Interfaces;
using WebApi.DAL.Repository.Interfaces;
using WebApi.Entity.Models;
using WebApi.Entity.DTOs;


namespace WebApi.BLL.Repository.Implementacion
{
    public class ProductoServicio : IProductoServicio
    {
        private readonly IProductoRepositorio _productoRepositorio;
        private readonly ILogger<ProductoServicio> _logger;

        public ProductoServicio(IProductoRepositorio productoRepositorio, ILogger<ProductoServicio> logger)
        {
            _productoRepositorio = productoRepositorio;
            _logger = logger;
        }

        public async Task<List<ProductoDTO>> ObtenerTodos()
        {
            try
            {
                var productos = await _productoRepositorio.ObtenerTodos();
                return productos.Select(p => new ProductoDTO
                {
                    Nombre = p.Nombre,
                    Marca = p.Marca,
                    Precio = (decimal)p.Precio,
                    FechaCreacion = p.FechaCreacion
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los productos");
                return new List<ProductoDTO>();
            }
        }

        public async Task<ProductoDTO> ObtenerProductoPorNombreYMarca(string nombre, string marca)
        {
            var productoExistente = await _productoRepositorio.ObtenerProductoPorNombreYMarca(nombre, marca);
            if (productoExistente != null)
            {
                // Convertir el producto existente a ProductoDTO
                return new ProductoDTO
                {
                    Nombre = productoExistente.Nombre,
                    Marca = productoExistente.Marca
                    // Asignar otros campos según sea necesario
                };
            }
            return null; // O lanza una excepción si prefieres manejarlo de otra manera
        }       

        public async Task<Producto> ObtenerProductoPorId(int id)
        {
            try
            {
                var producto = await _productoRepositorio.ObtenerProductoPorId(id);
                if (producto == null)
                {
                    // Aquí podrías devolver un mensaje o manejar la lógica si no se encuentra el producto
                    return null;
                }
                return producto;
            }
            catch (Exception ex)
            {
                // Aquí podrías manejar las excepciones, o loggearlas si es necesario
                throw new ApplicationException($"Error al obtener el producto con ID {id}", ex);
            }
        }

        

        public async Task<bool> GuardarProductoAsync(ProductoDTO productoDTO)
        {
            try
            {
                var producto = new Producto
                {
                    Nombre = productoDTO.Nombre,
                    Marca = productoDTO.Marca,
                    Precio = productoDTO.Precio,
                    FechaCreacion = productoDTO.FechaCreacion
                };
                return await _productoRepositorio.GuardarProductoAsync(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el producto");
                return false;
            }
        }

        public async Task<bool> GuardarCambiosAsync()
        {
            try
            {
                return await _productoRepositorio.GuardarCambiosAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar los cambios");
                return false;
            }
        }

        public async Task<bool> ActualizarProductoAsync(int id, ProductoDTO productoDTO)
        {
            try
            {
                if (!await _productoRepositorio.ProductoExisteAsync(id))
                {
                    return false;
                }

                var producto = new Producto
                {
                    IdProducto = id,
                    Nombre = productoDTO.Nombre,
                    Marca = productoDTO.Marca,
                    Precio = productoDTO.Precio,
                    FechaCreacion = productoDTO.FechaCreacion
                };

                // Llamas al método de repositorio que ya maneja la transacción
                return await _productoRepositorio.ActualizarProductoAsync(id, producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto");
                return false;
            }
        }

        public async Task<bool> EliminarProductoAsync(int id)
        {
            try
            {
                if (!await _productoRepositorio.ProductoExisteAsync(id))
                {
                    return false;
                }

                // Llamas al método de repositorio que ya maneja la transacción
                return await _productoRepositorio.EliminarProductoAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto");
                return false;
            }
        }

        public async Task<(bool EsValido, string MensajeError)> ValidarProducto(ProductoDTO productoDTO)
        {
            // Verificar que el número no tenga caracteres extraños (aunque ya es decimal)
            string precioStr = productoDTO.Precio.ToString();
            if (!decimal.TryParse(precioStr, out decimal precioDecimal))
            {
                throw new ArgumentException("El precio debe ser un número válido sin caracteres especiales.");
            }

            // Asegurar máximo 2 decimales
            if (Decimal.Round(precioDecimal, 2) != precioDecimal)
            {
                throw new ArgumentException("El precio solo puede tener hasta dos decimales.");
            }

            if (productoDTO == null)
            {
                return (false, "El producto no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(productoDTO.Nombre))
            {
                return (false, "El nombre del producto es obligatorio.");
            }

            if (productoDTO.Precio <= 0)
            {
                return (false, "El precio debe ser mayor a 0.");
            }

            return (true, "");
        }


    }
}
