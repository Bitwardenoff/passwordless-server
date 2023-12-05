using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Testcontainers.MsSql;
using Xunit;

namespace Passwordless.Api.IntegrationTests;

public class PasswordlessApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string OriginUrl = "https://bitwarden.com/products/passwordless/";
    public const string RpId = "bitwarden.com";

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();

    public FakeTimeProvider TimeProvider { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseSetting("ConnectionStrings:sqlite:api", string.Empty)
            .UseSetting("ConnectionStrings:mssql:api", _dbContainer.GetConnectionString())
            .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders())
            .ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IHostedService));
                services.RemoveAll(typeof(TimeProvider));
                services.AddSingleton<TimeProvider>(TimeProvider);
            });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Perform migrations and make sure the API is ready to receive requests
        using var httpClient = this.CreateClient();
        using var response = await httpClient.GetAsync("/");
        response.EnsureSuccessStatusCode();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}