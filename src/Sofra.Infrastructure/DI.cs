using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sofra.Domain.Entities;
using Sofra.Infrastructure.Persistent;
using Sofra.Infrastructure.Persistent.Interceptors;

namespace Sofra.Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<AuditableEntityInterceptor>();

            services.AddDbContext<SofraDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
            });

            services.AddIdentityCore<AppUser>()
                .AddRoles<AppRole>()
                .AddEntityFrameworkStores<SofraDbContext>();

            return services;
        }
    }
}
