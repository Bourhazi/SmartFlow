using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Infrastructure.Persistence;
using SmartFlow.Infrastructure.Persistence.Repositories;
using SmartFlow.Infrastructure.Persistence.Queries;

using SmartFlow.Infrastructure.FileStorage;

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

        return services;
    }
}