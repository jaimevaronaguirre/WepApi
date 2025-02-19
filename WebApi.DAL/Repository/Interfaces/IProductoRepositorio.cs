using WebApi.Entity.Models;

namespace WebApi.DAL.Repository.Interfaces
{
    public interface IProductoRepositorio
    {       
        Task<List<Producto>> ObtenerTodos();
        Task<bool> ProductoExisteAsync(int Id);
        Task<Producto> ObtenerProductoPorId(int id);
        Task<bool> GuardarProductoAsync(Producto producto);
        Task<bool> ActualizarProductoAsync(int id, Producto producto);
        Task<bool> EliminarProductoAsync(int id);
        Task<bool> GuardarCambiosAsync();
        Task<Producto> ObtenerProductoPorNombreYMarca(string nombre, string marca);
    }
}
