using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sofra.Application.Constants;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Identity;

/// <summary>
/// Creates the platform administrator account at startup if it does not exist yet.
/// Email/password come from the "Admin" configuration section (defaults provided).
/// </summary>
public class AdminSeedingService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<AdminSeedingService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var email = configuration["Admin:Email"] ?? "admin@sofra.test";
        var password = configuration["Admin:Password"] ?? "Admin123!";

        using var scope = scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        if (await userManager.FindByEmailAsync(email) is not null) return;

        if (!await roleManager.RoleExistsAsync(Roles.Administrator))
            await roleManager.CreateAsync(new AppRole { Name = Roles.Administrator });

        var admin = new AppUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to create admin account: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, Roles.Administrator);
        logger.LogInformation("Administrator account created: {Email}", email);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
