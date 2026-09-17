using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Infrastructure.Persistence;
using SmartFlow.Infrastructure.Persistence.Repositories;
using SmartFlow.Infrastructure.Persistence.Queries;
using Microsoft.AspNetCore.Identity;
using SmartFlow.Application.Authentication;
using SmartFlow.Infrastructure.Identity;
using SmartFlow.Infrastructure.FileStorage;
using SmartFlow.Application.Users;



namespace SmartFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(
            "DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
        }

        services.AddDbContext<SmartFlowDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<SmartFlowDbContext>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserAdministrationService, UserAdministrationService>();

        services.AddScoped<IRequestRepository, RequestRepository>();

        services.AddScoped<IUnitOfWork>(serviceProvider => 
            serviceProvider.GetRequiredService<SmartFlowDbContext>());
        services.AddSingleton<IFileStorage>(serviceProvider =>
        {
            var hostEnvironment = serviceProvider
                .GetRequiredService<IHostEnvironment>();

            var relativeRootPath =
                configuration["FileStorage:RootPath"] ?? "storage";

            var rootPath = Path.Combine(
                hostEnvironment.ContentRootPath,
                relativeRootPath);

            return new LocalFileStorage(rootPath);
        });

        services.AddScoped<IRequestReadService, RequestReadService>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        return services;
    }
}