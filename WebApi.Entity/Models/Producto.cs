using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entity.Models;

public partial class Producto
{
    [Key]
    public int IdProducto { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El número máximo de caracteres es de 100!")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La marca es obligatorio")]
    [MaxLength(100, ErrorMessage = "El número máximo de caracteres es de 100!")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }
    [Required]    
    public DateTime FechaCreacion { get; set; }
}
