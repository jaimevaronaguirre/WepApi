using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Entity.DTOs;
using WebApi.Entity.Models;

namespace WebApi.DAL.Repository.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<Usuario> ObtenerPorEmailYPaswordAsync(string email, string password);
        Task<bool> EmailExisteAsync(string email);
        Task<bool> UsuarioExisteAsync(int Id);
        Task<bool> GuardarUsuarioAsync(Usuario usuario);     
        Task<bool> GuardarCambiosAsync();        
    }
}