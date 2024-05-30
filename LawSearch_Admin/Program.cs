using LawSearch_Admin;
using LawSearch_Admin.Interfaces;
using LawSearch_Admin.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using LawSearch_Admin.Extensions;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["BackendApiUrl"] ?? "http://localhost") });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IArticalAdminService, ArticalAdminService>();
builder.Services.AddScoped<IConceptAdminService, ConceptAdminService>();
builder.Services.AddScoped<IKeyPhraseAdminService, KeyPhraseAdminService>();
builder.Services.AddScoped<ILawDocAdminService, LawDocAdminService>();
builder.Services.AddScoped<ISearchAdminService, SearchAdminService>();
builder.Services.AddScoped<IUserAdminService, UserAdminService>();
builder.Services.AddScoped<IHistoryAdminService, HistoryAdminService>();
builder.Services.AddScoped<ICheckUserHasAccessService, ChechUserHasAccessService>();
builder.Services.AddBlazoredLocalStorage();
await builder.Build().RunAsync();
