using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Common;
using Ductus.FluentDocker.Services;
using Xunit;

namespace Api.Tests.IntegrationTests;

public class DockerFixture : IAsyncLifetime
{
    private ICompositeService _containerService;

    public string ApiBaseAddress = "http://127.0.0.1:5009";

    public async Task InitializeAsync()
    {
        try
        {
            
            // Set up Docker Compose
            _containerService = new Builder()
                .UseContainer()
                .UseCompose()
                .FromFile("E:\\projects\\test\\microservice-advertising-system\\docker-compose.yml")
                .RemoveOrphans()
                .WaitForPort("api-gateway.app.api", "5009")
                .Build()
                .Start();
        }
        catch (Exception e)
        {
            Log(e);
            throw;
        }

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        try
        {
            // Stop Docker containers
            _containerService.Stop();
        }
        catch (Exception e)
        {
            Log(e);
            throw;
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// log exception stacktrace and message
    /// </summary>
    /// <param name="e"></param>
    private void Log(Exception e)
    {
        Logger.Log(e.Message);
        Logger.Log(e.StackTrace);
    }
}