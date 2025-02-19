using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.BLL.Repository.Implementacion;
using WebApi.BLL.Repository.Interfaces;
using WebApi.BLL.Utilidades;
using WebApi.DAL.Repository.Implementacion;
using WebApi.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebApi.Entity.EntityMapper;
using WebApi.DAL.DataContext;

namespace WebApi.IOC
{
    public static class ConfigInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Cadena de Conexión
            services.AddDbContext<AplicationDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ConexionSQL"));
            });

            // Agregar servicios singleton
            services.AddSingleton<EncriptadorSHA256>();
            services.AddSingleton<GeneradorJWT>();

            // Registrar repositorios
            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            services.AddScoped<IProductoRepositorio, ProductoRepositorio>();

            // Registrar servicios
            services.AddScoped<IUsuarioServicio, UsuarioServicio>();
            services.AddScoped<IProductoServicio, ProductoServicio>();

            // Configurar autenticación JWT
            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;
                config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(configuration["Jwt:key"]!))
                };
            });

            // Registrar AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<UsuarioMapper>();
            });
            services.AddAutoMapper(typeof(ConfigInjection));

            // Configurar políticas de CORS
            services.AddCors(options =>
            {
                options.AddPolicy("NewPolicy", app =>
                {
                    app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
