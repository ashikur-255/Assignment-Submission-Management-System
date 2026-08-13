using System.Net;
using System.Text.Json;
using AssignmentManagementSystem.Core.DTOs;
using MongoDB.Driver;

namespace AssignmentManagementSystem.API.Middleware;

public sealed class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception for {Method} {Path}",
                context.Request.Method,
                context.Request.Path
            );

            var (status, message) = ex switch
            {
                KeyNotFoundException =>
                    (HttpStatusCode.NotFound, ex.Message),

                ArgumentException =>
                    (HttpStatusCode.BadRequest, ex.Message),

                InvalidOperationException =>
                    (HttpStatusCode.BadRequest, ex.Message),

                MongoWriteException mongoEx
                    when mongoEx.WriteError?.Category ==
                         ServerErrorCategory.DuplicateKey =>
                    (HttpStatusCode.Conflict,
                     "A record with the same unique value already exists."),

                MongoBulkWriteException mongoEx
                    when mongoEx.WriteErrors.Any(x =>
                        x.Category == ServerErrorCategory.DuplicateKey) =>
                    (HttpStatusCode.Conflict,
                     "A record with the same unique value already exists."),

                UnauthorizedAccessException =>
                    (HttpStatusCode.Forbidden, "You are not authorized to perform this action."),

                _ =>
                    (HttpStatusCode.InternalServerError,
                     "An unexpected server error occurred.")
            };

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>(
                false,
                message
            );

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }
    }
}