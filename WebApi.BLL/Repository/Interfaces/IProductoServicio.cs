using WebApi.Entity.DTOs;
using WebApi.Entity.Models;

namespace WebApi.BLL.Repository.Interfaces
{
    public interface IProductoServicio
    {       
        Task<List<ProductoDTO>> ObtenerTodos();
        Task<ProductoDTO> ObtenerProductoPorNombreYMarca(string nombre, string marca);
        Task<Producto> ObtenerProductoPorId(int id);
        Task<bool> GuardarProductoAsync(ProductoDTO productoDTO);
        Task<bool> ActualizarProductoAsync(int id, ProductoDTO productoDTO);
        Task<bool> EliminarProductoAsync(int id);
        Task<bool> GuardarCambiosAsync();
        Task<(bool EsValido, string MensajeError)> ValidarProducto(ProductoDTO productoDTO);
    }
}
