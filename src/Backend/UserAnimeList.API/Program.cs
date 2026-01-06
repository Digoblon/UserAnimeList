using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using UserAnimeList.Application;
using UserAnimeList.Filters;
using UserAnimeList.Infrastructure;
using UserAnimeList.Middleware;

const string AUTHENTICATION_TYPE = "Bearer";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    //options.OperationFilter<IdsFilter>();
    
    options.AddSecurityDefinition(AUTHENTICATION_TYPE, new OpenApiSecurityScheme {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Scheme = AUTHENTICATION_TYPE,
        Name = "Authorization",
        Description = @"JWT Authorization header using the Bearer scheme. 
                        Enter 'Bearer'  [space] and then your token in the text input below.
                        Example: 'Bearer {token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement {{
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = AUTHENTICATION_TYPE
            },
            Scheme = "oauth2",
            Name = AUTHENTICATION_TYPE,
            In = ParameterLocation.Header
        },
        Array.Empty<string>()
    }});
});

builder.Services.AddSwaggerGen();

builder.Services.AddMvc(options =>options.Filters.Add<ExceptionFilter>());

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
//builder.Services.AddScoped<ITokenProvider, HttpContextTokenValue>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    //app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();



await app.RunAsync();
