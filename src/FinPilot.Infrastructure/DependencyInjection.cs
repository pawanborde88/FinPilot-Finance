using FinPilot.Application.Common.Interfaces;
using FinPilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinPilot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var rawConnection = configuration.GetConnectionString("DefaultConnection")
            ?? configuration["DATABASE_URL"]
            ?? throw new InvalidOperationException("No database connection string found in 'DefaultConnection' or 'DATABASE_URL'.");

        var isPostgreSql = rawConnection.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
                           rawConnection.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
                           rawConnection.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(configuration["DB_PROVIDER"], "PostgreSQL", StringComparison.OrdinalIgnoreCase);

        if (isPostgreSql)
        {
            var npgsqlConnectionString = ParsePostgreSqlConnectionString(rawConnection);
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(npgsqlConnectionString, b =>
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(rawConnection, b =>
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static string ParsePostgreSqlConnectionString(string connectionString)
    {
        if (!connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : string.Empty;
        var database = uri.AbsolutePath.TrimStart('/');
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;

        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
}
