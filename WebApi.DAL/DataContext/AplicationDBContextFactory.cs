using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WebApi.DAL.DataContext
{
    public class AplicationDBContextFactory : IDesignTimeDbContextFactory<AplicationDBContext>
    {
        public AplicationDBContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var connectionSting = configuration.GetConnectionString("ConexionSQL");

            var optionsBuilder = new DbContextOptionsBuilder<AplicationDBContext>();
            optionsBuilder.UseSqlServer(connectionSting);

            return new AplicationDBContext(optionsBuilder.Options);
        }
    }
}
