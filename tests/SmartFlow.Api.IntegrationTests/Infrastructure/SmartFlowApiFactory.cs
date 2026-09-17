using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartFlow.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace SmartFlow.Api.IntegrationTests.Infrastructure;

public sealed class SmartFlowApiFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder("postgres:17-alpine")
            .WithDatabase("smartflow_tests")
            .WithUsername("smartflow_test_user")
            .WithPassword("SmartFlowTest2026")
            .Build();

    private readonly string _fileStorageRoot = Path.Combine(
        Path.GetTempPath(),
        "SmartFlowTests",
        Guid.NewGuid().ToString("N"));

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();

        if (Directory.Exists(_fileStorageRoot))
        {
            Directory.Delete(_fileStorageRoot, recursive: true);
        }

        await base.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<SmartFlowDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.UseSetting(
            "ConnectionStrings:DefaultConnection",
            _postgresContainer.GetConnectionString());

        builder.UseSetting(
            "FileStorage:RootPath",
            _fileStorageRoot);
    }
}