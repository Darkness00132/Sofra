using Asp.Versioning;
using Scalar.AspNetCore;
using Sofra.Api.Services;
using Sofra.Application.Common.Interfaces;

namespace Sofra.Api;

public static class DI
{
    public static IServiceCollection AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        services.AddScoped<ICurrentUser, CurrentUser>();

        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.ReportApiVersions = true;
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })
            .AddOpenApi();

        return services;
    }

    public static WebApplication AddDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        app.MapOpenApi()
            .WithDocumentPerVersion();

        app.MapScalarApiReference(options =>
        {
            var versions = app.DescribeApiVersions()
                .OrderByDescending(x => x.ApiVersion)
                .ToList();

            for (var i = 0; i < versions.Count; i++)
            {
                var version = versions[i];

                options.AddDocument(
                    version.GroupName,
                    $"Sofra API {version.GroupName.ToUpperInvariant()}",
                    isDefault: i == 0);
            }
        });

        return app;
    }
}
