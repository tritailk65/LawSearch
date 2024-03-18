using LawSearch_API.Extensions;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using LawSearch_Core.Services;
using Microsoft.Data.SqlClient;
using Serilog;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
var appSettings = new AppSetting();

builder.Services.AddControllers().AddNewtonsoftJson(x =>
    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

//Add database connection
builder.Services.AddTransient<IDbConnection>(options =>
   new SqlConnection(configuration.GetSection("DbConnection").Value)
);

//Serilog with log rest API
var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

//Cors setting
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:7201");
        });
});

// Add services to the container (DI).
builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddScoped<IArticalService, ArticalService>();
builder.Services.AddScoped<IConceptService, ConceptService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(gen =>
{
    gen.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "LawSearchAPI", Version = "v1.0" });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // For mobile apps, allow http traffic.
    app.UseHttpsRedirection();
}

app.UseMiddleware(typeof(GlobalErrorHandlingMiddleware));

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Value}/{id?}");
});

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "LawSearch Endpoint");
});

app.Run();
