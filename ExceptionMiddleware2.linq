var app = WebApplication.Create();
app.UseMiddleware<ExceptionMiddleware>();

//app.UseExceptionHandler(c => c.Run(async context =>
//{
//	var exception = context.Features
//		.Get<IExceptionHandlerPathFeature>()
//		.Error;
//	var response = new { error = exception.Message };
//	await context.Response.WriteAsJsonAsync(response);
//}));

app.Run(async (context) =>
{
	int x = 0;
	int y = 8 / x;
	await context.Response.WriteAsync($"Result = {y}");
});

app.Run();

class ExceptionMiddleware 
{
	private readonly ILogger<ExceptionMiddleware> _logger;
	
	private readonly RequestDelegate _next;

	public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
	{
		_logger = logger;
		_next = next;
	}

	public async Task Invoke(HttpContext context)//, RequestDelegate next)
	{
		try
		{
			await _next(context);					
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, exception.Message);
			await HandleExceptionAsync(exception, context);
		}
	}

	private async Task HandleExceptionAsync(Exception exception, HttpContext context)
	{
		var (statusCode, error) = exception switch
		{
			CustomException => (StatusCodes.Status400BadRequest,
				new Error(exception.GetType().Name.Replace("_exception", string.Empty), exception.Message)),
			_ => (StatusCodes.Status500InternalServerError, new Error("error", "There was an error."))
		};

		context.Response.StatusCode = statusCode;
		await context.Response.WriteAsJsonAsync(error);
	}

	private record Error(string Code, string Reason);


}

public abstract class CustomException : Exception
{
	protected CustomException(string message) : base(message)
	{
	}
}
