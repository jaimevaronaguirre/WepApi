using Microsoft.EntityFrameworkCore;
using WebApi.Entity.Models;

namespace WebApi.DAL.DataContext
{
    public class AplicationDBContext : DbContext
    {
        public AplicationDBContext(DbContextOptions<AplicationDBContext> options) : base(options)
        {            
        }

        // Aquí pasar todas las entidades (Modelos)
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
    }
}
