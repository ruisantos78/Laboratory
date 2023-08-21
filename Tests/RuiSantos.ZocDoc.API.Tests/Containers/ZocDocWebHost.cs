using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using RuiSantos.ZocDoc.Api;

namespace RuiSantos.ZocDoc.API.Tests.Containers;

public sealed class ZocDocWebHost: IDisposable
{
	public TestServer Server { get; }

	public ZocDocWebHost(string connectionString)
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?>()
			{
				{ "DATABASE_DYNAMO" , connectionString },
				{ "ASPNETCORE_ENVIRONMENT", "Development" },
				{ "ASPNETCORE_URLS" , "http://+:80" }
            })
			.Build();

		var builder = new WebHostBuilder()
			.UseEnvironment("Test")
			.UseConfiguration(configuration)
			.UseStartup<Startup>();

		this.Server = new TestServer(builder);
	}

    public void Dispose()
    {
		Server.Dispose();
    }
}

