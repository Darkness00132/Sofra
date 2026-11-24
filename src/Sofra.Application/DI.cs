using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Sofra.Application.Behaviours;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Services;

namespace Sofra.Application;

public static class DI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DI).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IRestaurantOwnershipGuard, RestaurantOwnershipGuard>();

        services.AddMapster();

        // this package's AddMapster does not auto-discover IRegister classes — scan explicitly
        TypeAdapterConfig.GlobalSettings.Scan(typeof(DI).Assembly);

        return services;
    }
}