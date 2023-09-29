using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RuiSantos.Labs.Client;
using Blazorise;
using Blazorise.Material;
using Blazorise.Icons.Material;
using Blazing.Mvvm;
using Blazorise.LoadingIndicator;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var graphqlUrl = Environment.GetEnvironmentVariable("RUISANTOS_LABS_GRAPHQL_URL") ?? "http://localhost:8001/graphql";

builder.Services
    .AddLabsClient()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(graphqlUrl));

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = false;
    })
    .AddMaterialProviders()
    .AddMaterialIcons()
    .AddLoadingIndicator();

builder.Services
    .AddBlazoredLocalStorage()
    .AddMvvmNavigation();

builder.Services
    .RegisterServices()
    .RegisterViewModels();

await builder.Build().RunAsync();
