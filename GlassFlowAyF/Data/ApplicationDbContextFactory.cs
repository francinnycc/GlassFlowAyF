using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GlassFlowAyF.Data;

// Permite generar migraciones y scripts sin conectarse a una base de datos.
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddUserSecrets<ApplicationDbContextFactory>(optional: true)
            .AddEnvironmentVariables().Build();
        var connection = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=glassflow_af;User=root";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 0))).Options;
        return new ApplicationDbContext(options);
    }
}
