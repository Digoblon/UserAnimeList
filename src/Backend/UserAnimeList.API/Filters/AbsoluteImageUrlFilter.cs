using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UserAnimeList.Filters;

public sealed class AbsoluteImageUrlFilter : IAsyncActionFilter
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> ImageUrlPropsCache = new();

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next();

        if (executed.Result is not ObjectResult objectResult || objectResult.Value is null)
            return;

        var httpRequest = context.HttpContext.Request;
        var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}";

        FixImageUrls(objectResult.Value, baseUrl);
    }

    private static void FixImageUrls(object value, string baseUrl)
    {
        if (value is IEnumerable enumerable and not string)
        {
            foreach (var item in enumerable)
            {
                if (item is not null)
                    FixImageUrls(item, baseUrl);
            }
            return;
        }

        ApplyImageUrls(value, baseUrl);

        var props = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            if (!prop.CanRead) continue;

            var propValue = prop.GetValue(value);
            if (propValue is null) continue;

            if (propValue is IEnumerable childEnumerable and not string)
            {
                foreach (var child in childEnumerable)
                {
                    if (child is not null)
                        FixImageUrls(child, baseUrl);
                }
            }
        }
    }

    private static void ApplyImageUrls(object obj, string baseUrl)
    {
        var type = obj.GetType();

        var urlProps = ImageUrlPropsCache.GetOrAdd(type, t =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
             .Where(p => p.CanRead && p.CanWrite)
             .Where(p => p.PropertyType == typeof(string))
             .Where(p => p.Name.EndsWith("ImageUrl", StringComparison.OrdinalIgnoreCase))
             .ToArray()
        );

        if (urlProps.Length == 0)
            return;

        foreach (var prop in urlProps)
        {
            var current = (string?)prop.GetValue(obj);

            if (string.IsNullOrWhiteSpace(current))
                continue;

            if (current.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                current.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                continue;

            var normalized = current.TrimStart('/').Replace("\\", "/");
            prop.SetValue(obj, $"{baseUrl}/{normalized}");
        }
    }
}