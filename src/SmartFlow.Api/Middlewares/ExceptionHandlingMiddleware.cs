using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFlow.Domain.Exceptions;
using SmartFlow.Application.Common.Exceptions;
namespace SmartFlow.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "An unhandled exception occurred for request {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            await WriteProblemDetailsAsync(context, exception);
        }
    }

    private static Task WriteProblemDetailsAsync(
        HttpContext context,
        Exception exception)
    {
        var problemDetails = CreateProblemDetails(context, exception);

        context.Response.StatusCode = problemDetails.Status!.Value;
        context.Response.ContentType = "application/problem+json";

        return context.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken: context.RequestAborted);
    }




    private static ProblemDetails CreateProblemDetails(
        HttpContext context,
        Exception exception)
    {
        var problemDetails = exception switch
        {
            ValidationException validationException =>
                CreateValidationProblemDetails(context, validationException),
            UnauthorizedAccessException =>
            CreateProblemDetails(
                context,
                StatusCodes.Status401Unauthorized,
                "Authentication failed",
                "Email or password is invalid."),


            InvalidOperationException invalidOperationException =>
            CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Operation failed",
                invalidOperationException.Message),


            ForbiddenAccessException forbiddenAccessException =>
            CreateProblemDetails(
                context,
                StatusCodes.Status403Forbidden,
                "Access denied",
                forbiddenAccessException.Message),
            
            DomainException domainException =>
                CreateProblemDetails(
                    context,
                    StatusCodes.Status400BadRequest,
                    "Business rule violation",
                    domainException.Message),

            DbUpdateConcurrencyException =>
                CreateProblemDetails(
                    context,
                    StatusCodes.Status409Conflict,
                    "Concurrency conflict",
                    "This request was changed by another user. Refresh and try again."),

            _ => CreateProblemDetails(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "An unexpected error occurred.")
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        return problemDetails;
    }

    private static ProblemDetails CreateValidationProblemDetails(
        HttpContext context,
        ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .ToArray());

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Detail = "One or more validation errors occurred.",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["errors"] = errors;

        return problemDetails;
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
    }
}