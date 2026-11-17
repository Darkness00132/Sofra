using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sofra.Infrastructure.Persistent;

/// <summary>
/// Lets dotnet-ef build migrations without running the API host (whose startup needs a
/// database and HTTP pipeline). Uses the same SQL Server configuration as AddInfrastructure.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SofraDbContext>
{
    public SofraDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SofraDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=Sofra;Trusted_Connection=True;TrustServerCertificate=True;");

        return new SofraDbContext(optionsBuilder.Options);
    }
}
