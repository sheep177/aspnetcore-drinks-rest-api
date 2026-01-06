using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Drinks.API.Filters;

public class ETagFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult &&
            objectResult.Value != null &&
            context.HttpContext.Request.Method == HttpMethods.Get)
        {
            var json = JsonSerializer.Serialize(objectResult.Value);
            var etag = GenerateETag(json);

            var requestEtags = context.HttpContext.Request.Headers.IfNoneMatch;

            if (requestEtags.Any(h => h == etag))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
                return;
            }

            context.HttpContext.Response.Headers.ETag = etag;
        }

        await next();
    }

    private static string GenerateETag(string content)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
        return $"\"{Convert.ToBase64String(hash)}\"";
    }
}