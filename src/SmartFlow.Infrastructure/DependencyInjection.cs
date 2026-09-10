using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Infrastructure.Persistence;
using SmartFlow.Infrastructure.Persistence.Repositories;

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

        return services;
    }
}