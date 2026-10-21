using Microsoft.Extensions.DependencyInjection;

namespace FinPilot.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add application-layer dependencies (e.g., MediatR, AutoMapper, FluentValidation) here
        return services;
    }
}
