using Sofra.Application;
using Sofra.Infrastructure;
using Sofra.Api;
using Sofra.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.AddDocumentation();

app.Run();