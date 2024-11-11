using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Service.Commons.Middlewares;

public class JsonNormalizationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (IsJsonRequest(context))
        {
            context.Request.EnableBuffering();

            var bodyAsString = await ReadRequestBodyAsync(context.Request);
            var normalizedJson = NormalizeJson(bodyAsString);

            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(normalizedJson));
        }

        await _next(context);
    }

    private static bool IsJsonRequest(HttpContext context) =>
        context.Request.ContentType?.Contains("application/json") == true &&
        (context.Request.Method == HttpMethods.Post ||
         context.Request.Method == HttpMethods.Put ||
         context.Request.Method == HttpMethods.Patch);

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var bodyAsString = await reader.ReadToEndAsync();
        request.Body.Position = 0;  
        return bodyAsString;
    }

    private static string NormalizeJson(string json)
    {
        using var jsonDoc = JsonDocument.Parse(json);
        var options = new JsonSerializerOptions { WriteIndented = false };
        var normalizedJson = JsonSerializer.Serialize(jsonDoc, options);

        return Regex.Replace(normalizedJson, @"\s+", " ")
                    .Replace("\" ", "\"")
                    .Replace(" \"", "\"");
    }
}
