using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Entity.DTOs;


namespace WebApi.BLL.Repository.Interfaces
{
    public  interface IUsuarioServicio
    {
        Task<(bool, string)> RegistrarUsuarioAsync(UsuarioDTO usuarioDto);        
        Task<(bool Exito, string Mensaje, string Token)> LoginAsync(LoginDTO loginDto);
        
    }
}
