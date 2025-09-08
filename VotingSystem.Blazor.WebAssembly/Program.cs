using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VotingSystem.Blazor.WebAssembly;
using VotingSystem.Blazor.WebAssembly.Infrastructure;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7287") });
builder.Services.AddBlazorServices(builder.Configuration);

await builder.Build().RunAsync();
