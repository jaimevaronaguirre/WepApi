using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entity.Models;

public partial class Usuario
{
    [Key]
    public int IdUsuario { get; set; }
    [Required]
    public string? Nombre { get; set; }
    [Required]
    public string? Correo { get; set; }
    [Required]
    public string? Clave { get; set; }
    [Required]
    public DateTime FechaCreacion { get; set; }
}
