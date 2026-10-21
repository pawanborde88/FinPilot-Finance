using FinPilot.Application;
using FinPilot.Infrastructure;
using FinPilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add Clean Architecture layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Controller services
builder.Services.AddControllers();

// Configure Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure Database Schema is up to date on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        if (dbContext.Database.IsNpgsql())
        {
            logger.LogInformation("Applying PostgreSQL database schema initialization in Supabase...");
            var databaseCreator = dbContext.Database.GetService<IRelationalDatabaseCreator>();
            try
            {
                await databaseCreator.CreateTablesAsync();
                logger.LogInformation("PostgreSQL tables successfully created.");
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P07")
            {
                logger.LogInformation("PostgreSQL tables already exist.");
            }

            try
            {
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                        ""MigrationId"" character varying(150) NOT NULL,
                        ""ProductVersion"" character varying(32) NOT NULL,
                        CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY (""MigrationId"")
                    );
                    INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                    VALUES ('20261021122751_InitialPostgreSqlCreate', '10.0.10')
                    ON CONFLICT (""MigrationId"") DO NOTHING;");
            }
            catch (Exception historyEx)
            {
                logger.LogWarning(historyEx, "Unable to record initial migration history entry.");
            }
        }
        else if (dbContext.Database.IsSqlServer())
        {
            logger.LogInformation("Applying SQL Server database migrations...");
            await dbContext.Database.MigrateAsync();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Enable Swagger in Development and Production (e.g. Render)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FinPilot API v1");
    options.RoutePrefix = "swagger";
});

// Redirect root to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
