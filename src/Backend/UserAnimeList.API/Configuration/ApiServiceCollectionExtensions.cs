using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Exception;
using UserAnimeList.Filters;
using UserAnimeList.Token;

namespace UserAnimeList.Configuration;

public static class ApiServiceCollectionExtensions
{
    private const string AuthenticationType = "Bearer";

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(AuthenticationType, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Scheme = AuthenticationType,
                Name = "Authorization",
                Description = @"JWT Authorization header using the Bearer scheme. 
                        Enter 'Bearer'  [space] and then your token in the text input below.
                        Example: 'Bearer {token}'"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = AuthenticationType
                        },
                        Scheme = "oauth2",
                        Name = AuthenticationType,
                        In = ParameterLocation.Header
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddHttpContextAccessor();
        services.AddScoped<AuthenticatedUserFilter>();
        services.AddScoped<AbsoluteImageUrlFilter>();
        services.AddScoped<ITokenProvider, HttpContextTokenValue>();
        services.AddRouting(options => options.LowercaseUrls = true);
        services.Configure<RequestLocalizationOptions>(ConfigureLocalizationOptions);
        services.Configure<ApiBehaviorOptions>(ConfigureApiBehaviorOptions);

        return services;
    }

    private static void ConfigureLocalizationOptions(RequestLocalizationOptions options)
    {
        var supportedCultures = new[] { "en", "pt", "ja" };

        options.SetDefaultCulture("en");
        options.AddSupportedCultures(supportedCultures);
        options.AddSupportedUICultures(supportedCultures);
        options.RequestCultureProviders = [new AcceptLanguageHeaderRequestCultureProvider()];
    }

    private static void ConfigureApiBehaviorOptions(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = NormalizeErrors(context);
            return new BadRequestObjectResult(new ResponseErrorJson(errors));
        };
    }

    private static List<string> NormalizeErrors(ActionContext context)
    {
        if (IsMissingRequestBody(context))
            return [ResourceMessagesException.INVALID_REQUEST_BODY];

        var errors = new List<string>();

        foreach (var error in context.ModelState.Values.SelectMany(modelState => modelState.Errors))
        {
            if (error.Exception is JsonException)
            {
                errors.Add(ResourceMessagesException.INVALID_JSON);
                continue;
            }

            if (ActionExpectsBody(context) && IsBodyRequiredModelError(error))
            {
                errors.Add(ResourceMessagesException.INVALID_REQUEST_BODY);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
                errors.Add(error.ErrorMessage);
        }

        if (errors.Count == 0)
            errors.Add(ResourceMessagesException.INVALID_REQUEST_BODY);

        return errors.Distinct().ToList();
    }

    private static bool IsMissingRequestBody(ActionContext context)
    {
        if (!ActionExpectsBody(context))
            return false;

        var request = context.HttpContext.Request;

        return (request.ContentLength is null or 0) && !request.HasFormContentType;
    }

    private static bool ActionExpectsBody(ActionContext context)
    {
        return context.ActionDescriptor.Parameters
            .OfType<ControllerParameterDescriptor>()
            .Any(parameter =>
                parameter.BindingInfo?.BindingSource == BindingSource.Body ||
                parameter.ParameterInfo.GetCustomAttributes(typeof(FromBodyAttribute), true).Any());
    }

    private static bool IsBodyRequiredModelError(ModelError error)
    {
        if (string.IsNullOrWhiteSpace(error.ErrorMessage))
            return true;

        var message = error.ErrorMessage;

        return message.Contains("required", StringComparison.OrdinalIgnoreCase) &&
               message.Contains("body", StringComparison.OrdinalIgnoreCase);
    }
}
