using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using RuiSantos.ZocDoc.API.Tests.Containers;

namespace RuiSantos.ZocDoc.API.Tests;

public sealed partial class ZocDocAPIContainer : IAsyncDisposable
{
    private IFutureDockerImage? _image;
    private IContainer? _container;

    public async ValueTask DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }

        if (_image is not null)
        {
            await _image.DeleteAsync();
            await _image.DisposeAsync();
        }
    }

    public async Task BuildAsync()
    {
        _image = new ImageFromDockerfileBuilder()
            .WithName("zocdoc-api-test")
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("Tests/RuiSantos.ZocDoc.API.Tests/Dockerfile")
            .WithDeleteIfExists(true)
            .Build();

        await _image.CreateAsync();
        Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] zocdoc-api-test created!");
    }


    public async Task StartAsync(DynamoDbContainer dynamoDbContainer, INetwork network)
    {
        _container = new ContainerBuilder()
               .WithImage(_image)
               .DependsOn(dynamoDbContainer.GetContainer())
               .WithEnvironment("DATABASE_DYNAMO", dynamoDbContainer.GetConnectionString())
               .WithEnvironment("ASPNETCORE_URLS", "http://+:80")
               .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "true")
               .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
               .WithEnvironment("DOTNET_LOG_LEVEL", "Trace")
               .WithEnvironment("AWS_ACCESS_KEY_ID", "api")
               .WithEnvironment("AWS_SECRET_ACCESS_KEY", "secret")
               .WithPortBinding(80, true)
               .WithNetwork(network)
               .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(request =>
                        request.ForPath("/health")
                               .ForPort(80)
                               .ForStatusCode(HttpStatusCode.OK)
                    )
                )               
               .Build();

        await _container.StartAsync();
        Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] zocdoc-api-test server listening on: http://[::]:{_container.GetMappedPublicPort(80)}");
    }

    public Uri GetBaseAddress()
    {
        if (_container is null)
            throw new InvalidOperationException("Docker container is not ready for the database client.");

        return new Uri($"http://{_container.Hostname}:{_container.GetMappedPublicPort(80)}");
    }
}
