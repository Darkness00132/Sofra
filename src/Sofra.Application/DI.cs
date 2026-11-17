using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Sofra.Application.Behaviours;

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

        services.AddMapster();

        return services;
    }
}