using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RuiSantos.Labs.Client;
using Blazorise;
using Blazorise.Material;
using Blazorise.Icons.Material;
using Blazing.Mvvm;
using Blazorise.LoadingIndicator;
using Blazored.LocalStorage;
using RuiSantos.Labs.Client.Core.Mediators;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddLabsClient()
    .ConfigureHttpClient(client => client.BaseAddress = builder.Configuration.GetValue<Uri>("services"));

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
    .AddMvvm();

builder.Services
    .RegisterServices()
    .RegisterViewModels();

await builder.Build().RunAsync();
