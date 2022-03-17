using System;
using System.Net.Http;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.TodoItems.Shared.Exceptions;

namespace TodoApiDTO
{
    public static class ProblemDetailsConfiguration
    {
        public static void Configure(ProblemDetailsOptions options)
        {
            options.Map<NotFoundException>((context, exception) => new ProblemDetails
            {
                Detail = exception.ToString(),
                Title = exception.Message,
                Status = StatusCodes.Status404NotFound,
                Type = context.Request.Path
            });
            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
            options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        }
    }
}