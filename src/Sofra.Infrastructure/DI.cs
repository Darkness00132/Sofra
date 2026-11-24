using System.Text;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Features.Orders;
using Sofra.Domain.Entities;
using Sofra.Infrastructure.Authentication;
using Sofra.Infrastructure.Emailing;
using Sofra.Infrastructure.FileStorage;
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
                .AddEntityFrameworkStores<SofraDbContext>()
                // Email confirmation (register flow) and password reset need the
                // token providers AddIdentityCore does not register by default.
                .AddDefaultTokenProviders();

            // Match the Application validators: a 6-character minimum with no complexity
            // requirements, so Identity does not reject passwords the API already accepted.
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddHostedService<ApplyMigrationsHostedService>();
            services.AddHostedService<RolesSeedingService>();
            services.AddHostedService<AdminSeedingService>();

            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
            services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
            services.Configure<OrderFeesOptions>(configuration.GetSection(OrderFeesOptions.SectionName));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<Cloudinary>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
                return new Cloudinary(new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret));
            });
            services.AddScoped<IRestaurantApplicationRepository, RestaurantApplicationRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped<IFoodCategoryRepository, FoodCategoryRepository>();
            services.AddScoped<IFoodItemRepository, FoodItemRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<ITokenProvider, JwtTokenProvider>();
            services.AddScoped<IEmailSender, GmailEmailSender>();
            services.AddScoped<IFileStorage, CloudinaryFileStorage>();

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
