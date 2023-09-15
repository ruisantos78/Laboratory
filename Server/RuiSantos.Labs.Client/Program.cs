using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RuiSantos.Labs.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var graphqlUrl = Environment.GetEnvironmentVariable("RUISANTOS_LABS_GRAPHQL_URL") ?? "http://localhost:8001/graphql";

builder.Services
    .AddLabsClient()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(graphqlUrl));

builder.Services.RegisterServices();

await builder.Build().RunAsync();
