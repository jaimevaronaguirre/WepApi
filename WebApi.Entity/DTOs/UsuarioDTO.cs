using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Entity.DTOs
{
    public class UsuarioDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El número máximo de caracteres es de 100!")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Email es obligatorio")]
        [MaxLength(100, ErrorMessage = "El número máximo de caracteres es de 100!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La clave es obligatorio")]
        [MaxLength(16, ErrorMessage = "El número máximo de caracteres es de 16!")]
        public string Clave { get; set; }
    }
}
