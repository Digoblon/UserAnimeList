using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using UserAnimeList.Configuration;
using UserAnimeList.Application;
using UserAnimeList.Data.Seed;
using UserAnimeList.Infrastructure;
using UserAnimeList.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;

if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference();

    await Seeder.SeedAsync(app);
}

app.UseRequestLocalization(localizationOptions);
app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
