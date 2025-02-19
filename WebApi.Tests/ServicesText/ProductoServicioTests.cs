using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi.BLL.Repository.Implementacion;
using WebApi.DAL.Repository.Interfaces;
using WebApi.Entity.DTOs;
using WebApi.Entity.Models;

namespace WebApi.Tests.ServicesText
{
    public class ProductoServicioTests
    {
        private readonly Mock<IProductoRepositorio> _productoRepositorioMock;
        private readonly Mock<ILogger<ProductoServicio>> _loggerMock;
        private readonly ProductoServicio _productoServicio;

        public ProductoServicioTests()
        {
            _productoRepositorioMock = new Mock<IProductoRepositorio>();
            _loggerMock = new Mock<ILogger<ProductoServicio>>();
            _productoServicio = new ProductoServicio(_productoRepositorioMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ObtenerTodos_RetornaListaDeProductoDTO()
        {
            // Arrange
            var productos = new List<Producto>
        {
            new Producto { Nombre = "Producto1", Marca = "Marca1", Precio = 10, FechaCreacion = DateTime.Now },
            new Producto { Nombre = "Producto2", Marca = "Marca2", Precio = 20, FechaCreacion = DateTime.Now }
        };
            _productoRepositorioMock.Setup(repo => repo.ObtenerTodos()).ReturnsAsync(productos);

            // Act
            var result = await _productoServicio.ObtenerTodos();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Producto1", result[0].Nombre);
        }

        [Fact]
        public async Task GuardarProductoAsync_ValidarProductoDTO_RetornaTrue()
        {
            // Arrange
            var productoDTO = new ProductoDTO { Nombre = "NuevoProducto", Marca = "NuevaMarca", Precio = 30, FechaCreacion = DateTime.Now };
            _productoRepositorioMock.Setup(repo => repo.GuardarProductoAsync(It.IsAny<Producto>())).ReturnsAsync(true);

            // Act
            var result = await _productoServicio.GuardarProductoAsync(productoDTO);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ObtenerProductoPorId_ProductoExists_RetornaProducto()
        {
            // Arrange
            var producto = new Producto { IdProducto = 1, Nombre = "Producto1", Marca = "Marca1" };
            _productoRepositorioMock.Setup(repo => repo.ObtenerProductoPorId(1)).ReturnsAsync(producto);

            // Act
            var result = await _productoServicio.ObtenerProductoPorId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Producto1", result.Nombre);
        }

        // Agrega más pruebas unitarias según sea necesario
    }
}
