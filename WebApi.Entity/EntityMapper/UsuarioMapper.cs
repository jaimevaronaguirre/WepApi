using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WebApi.Entity.DTOs;
using WebApi.Entity.Models;

namespace WebApi.Entity.EntityMapper
{
    public class UsuarioMapper : Profile
    {
        public UsuarioMapper()
        {
            CreateMap<Usuario, LoginDTO>().ReverseMap();
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();

        }
    }
}
