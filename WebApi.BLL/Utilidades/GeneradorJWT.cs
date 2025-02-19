using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApi.Entity.Models;

namespace WebApi.BLL.Utilidades
{
    public class GeneradorJWT
    {
        private readonly IConfiguration _configuration;
        public GeneradorJWT(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string generarJWT(Usuario modelo)
        {
            //crear la informacion del usuario para token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,modelo.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email,modelo.Correo!),
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //Crear detalles del token
            var jwtConfig = new JwtSecurityToken(
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}
