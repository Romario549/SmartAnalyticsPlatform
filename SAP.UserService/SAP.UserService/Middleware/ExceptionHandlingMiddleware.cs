using System.Net;
using System.Text.Json;

namespace SAP.UserService.Api.Middleware;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;
	private readonly IHostEnvironment _env;

	public ExceptionHandlingMiddleware(
		RequestDelegate next,
		ILogger<ExceptionHandlingMiddleware> logger,
		IHostEnvironment env)
	{
		_next = next;
		_logger = logger;
		_env = env;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred: {Message}", ex.Message);
			await HandleExceptionAsync(context, ex);
		}
	}
	private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
	{
		context.Response.ContentType = "application/json";

		var response = ex switch
		{
			ArgumentException argEx => new
			{
				context.Response.StatusCode,
				Message = "Validtion Error",
				Errors = new[] { argEx.Message },
				Detail = argEx.Message
			},
			AggregateException aggEx => new
			{
				context.Response.StatusCode,
				Message = "Multiple validation errors",
				Errors = aggEx.InnerExceptions.Select(e => e.Message).ToArray(),
				Detail = string.Join("; ", aggEx.InnerExceptions.Select(e => e.Message))
			},
			_ => new
			{
				context.Response.StatusCode,
				Message = "An internal server error occurred",
				Errors = Array.Empty<string>(),
				Detail = "Please try again later"
			}
		};

		context.Response.StatusCode = ex switch
		{
			ArgumentException => (int)HttpStatusCode.BadRequest,
			AggregateException => (int)HttpStatusCode.BadRequest,
			_ => (int)HttpStatusCode.InternalServerError
		};

		var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
		var json = JsonSerializer.Serialize(response, options);

		await context.Response.WriteAsync(json);
	}
}
