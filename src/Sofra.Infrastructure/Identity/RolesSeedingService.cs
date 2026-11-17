using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sofra.Application.Constants;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Identity;

/// <summary>
/// Creates the platform roles at startup if they do not exist yet. Runs after
/// <see cref="ApplyMigrationsHostedService"/>, so the database is ready first.
/// </summary>
public class RolesSeedingService(IServiceScopeFactory scopeFactory, ILogger<RolesSeedingService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        foreach (var roleName in Roles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            await roleManager.CreateAsync(new AppRole { Name = roleName });
            logger.LogInformation("Seeded role {RoleName}", roleName);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
