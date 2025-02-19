using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.BLL.Repository.Interfaces;
using WebApi.Entity.DTOs;
using WebApi.Entity.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoServicio _productoServicio;

        public ProductoController(IProductoServicio productoServicio)
        {
            _productoServicio = productoServicio;
        }

        [HttpGet(Name = "Lista")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Lista()
        {
            try
            {
                var lista = await _productoServicio.ObtenerTodos();
                if (lista == null || !lista.Any())
                {
                    return NotFound("No se encontraron productos.");
                }
                return Ok(new { Value = lista });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}", Name = "ObtenerProductoPorId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _productoServicio.ObtenerProductoPorId(id);

            if (producto == null)
            {
                return NotFound(new { Message = $"Producto con ID {id} no encontrado." });
            }

            return Ok(producto);
        }

        [HttpPost]        
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearProducto([FromBody] ProductoDTO productoDTO)
        {
            try
            {
                var (esValido, mensajeError) = await _productoServicio.ValidarProducto(productoDTO);
                if (!esValido)
                {
                    return BadRequest(mensajeError);
                }

                // Verificar si el producto ya existe por Nombre y Marca
                var productoExiste = await _productoServicio.ObtenerProductoPorNombreYMarca(productoDTO.Nombre, productoDTO.Marca);
                if (productoExiste != null)
                {
                    return BadRequest("El producto ya existe con el mismo nombre y marca.");
                }

                var resultado = await _productoServicio.GuardarProductoAsync(productoDTO);
                if (!resultado)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo guardar el producto.");
                }

                return StatusCode(StatusCodes.Status201Created, "Producto creado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] ProductoDTO productoDTO)
        {
            try
            {
                if (productoDTO == null)
                {
                    return BadRequest(new { mensaje = "Debe proporcionar un producto válido." });

                    //return BadRequest("Datos de producto inválidos.");
                }

                var validacionResultado = await _productoServicio.ValidarProducto(productoDTO).ConfigureAwait(false);
                if (!validacionResultado.EsValido)
                {
                    return BadRequest(validacionResultado.MensajeError);
                }

                // Verificar si el producto ya existe por Nombre y Marca
                var productoExiste = await _productoServicio.ObtenerProductoPorNombreYMarca(productoDTO.Nombre, productoDTO.Marca);
                if (productoExiste != null)
                {
                    return BadRequest("El producto ya existe con el mismo nombre y marca.");
                }

                var resultado = await _productoServicio.ActualizarProductoAsync(id, productoDTO);
                if (!resultado)
                {
                    return NotFound("Producto no encontrado o no se pudo actualizar.");
                }

                return Ok("Producto actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            try
            {
                var resultado = await _productoServicio.EliminarProductoAsync(id);
                if (!resultado)
                {
                    return NotFound("Producto no encontrado o no se pudo eliminar.");
                }
                return Ok("Producto eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}

