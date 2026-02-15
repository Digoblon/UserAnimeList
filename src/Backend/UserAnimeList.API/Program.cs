using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using UserAnimeList.Application;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Domain.Services.DataSeed;
using UserAnimeList.Exception;
using UserAnimeList.Filters;
using UserAnimeList.Infrastructure;
using UserAnimeList.Middleware;
using UserAnimeList.Token;

const string authenticationType = "Bearer";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.AddSecurityDefinition(authenticationType, new OpenApiSecurityScheme {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Scheme = authenticationType,
        Name = "Authorization",
        Description = @"JWT Authorization header using the Bearer scheme. 
                        Enter 'Bearer'  [space] and then your token in the text input below.
                        Example: 'Bearer {token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement {{
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = authenticationType
            },
            Scheme = "oauth2",
            Name = authenticationType,
            In = ParameterLocation.Header
        },
        Array.Empty<string>()
    }});
});

builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<AuthenticatedUserFilter>();
builder.Services.AddScoped<AbsoluteImageUrlFilter>();


builder.Services.AddScoped<ITokenProvider, HttpContextTokenValue>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var rawErrors = context.ModelState
            .Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var errors = NormalizeErrors(rawErrors);

        return new BadRequestObjectResult(new ResponseErrorJson(errors));
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    //app.MapOpenApi();
    app.MapScalarApiReference();
    
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.SeedAsync();
}

app.UseMiddleware<CultureMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseStaticFiles();

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
return;

static List<string> NormalizeErrors(List<string> rawErrors)
{
    var errors = new List<string>();

    foreach (var error in rawErrors)
    {
        if (error.Contains("invalid start of a value", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(ResourceMessagesException.INVALID_JSON);
            continue;
        }

        if (error.Contains("The request field is required", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(ResourceMessagesException.INVALID_REQUEST_BODY);
            continue;
        }

        errors.Add(error);
    }

    return errors.Distinct().ToList();
}

public partial class Program
{
    protected Program() { }
}