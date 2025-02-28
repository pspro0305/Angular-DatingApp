using System;
using System.Net;
using System.Text.Json;
using DatingAppAPI.Errors;

namespace DatingAppAPI.Middleware;

public class ExceptionMiddleware(RequestDelegate next,
ILogger<ExceptionMiddleware> logger,
IHostEnvironment hostEnvironment)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
await next(httpContext);
        }
catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var response = hostEnvironment.IsDevelopment()?
            new ApiException(httpContext.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()):
            new ApiException(httpContext.Response.StatusCode, "Internal Server Error", null);
            var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}