using LawSearch_Admin;
using LawSearch_Admin.Interfaces;
using LawSearch_Admin.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["BackendApiUrl"]) });
builder.Services.AddScoped<IArticalAdminService, ArticalAdminService>();
builder.Services.AddScoped<IConceptAdminService, ConceptAdminService>();

await builder.Build().RunAsync();
