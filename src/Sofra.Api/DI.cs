using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Sofra.Api.Helper;
using Sofra.Api.Services;
using Sofra.Application.Common.Interfaces;
using System.Text.Json.Serialization;

namespace Sofra.Api;

public static class DI
{
    public static IServiceCollection AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                // the API contract speaks enums as strings everywhere (DB stores them as strings too)
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                // EF Core reads datetime2 back as DateTimeKind.Unspecified, which would
                // serialize without the UTC marker; the frontend contract requires "…Z"
                options.JsonSerializerOptions.Converters.Add(new UtcDateTimeJsonConverter());
            });

        // malformed JSON / binding failures must use the same envelope as every other failure
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(err => err.ErrorMessage).ToArray());

                var problemDetails = ApiErrorResponseFactory.Create(
                    context.HttpContext,
                    StatusCodes.Status400BadRequest,
                    "Validation Error",
                    "General.Validation",
                    "One or more validation errors occurred.",
                    errors);

                return new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status400BadRequest };
            };
        });

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
