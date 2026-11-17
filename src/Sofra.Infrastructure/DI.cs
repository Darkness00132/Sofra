using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Sofra.Application.Common.Interfaces;
using Sofra.Domain.Entities;
using Sofra.Infrastructure.Authentication;
using Sofra.Infrastructure.Emailing;
using Sofra.Infrastructure.Identity;
using Sofra.Infrastructure.Persistent;
using Sofra.Infrastructure.Persistent.Interceptors;
using Sofra.Infrastructure.Repositories;

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

            services.AddHostedService<ApplyMigrationsHostedService>();
            services.AddHostedService<RolesSeedingService>();

            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenProvider, JwtTokenProvider>();
            services.AddScoped<IEmailSender, GmailEmailSender>();

            return services;
        }

        /// <summary>Enables JWT bearer authentication using the "Jwt" configuration section.</summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException("The Jwt configuration section is missing.");

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            return services;
        }
    }
}
