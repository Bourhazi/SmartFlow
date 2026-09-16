using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SmartFlow.Application.Common.Behaviors;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly);
            configuration.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);
        services.AddScoped<RequestAccessGuard>();
        return services;    
    }
}