var builder = WebApplication.CreateBuilder();

//builder.Services.AddControllersWithViews().AddApplicationPart(typeof(HomeController).Assembly);
//builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseExceptionHandler(appError =>
			{
				appError.Run(async context =>
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					context.Response.ContentType = "application/json";
					var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
					if (contextFeature != null)
					{
						//Logger.LogError($"Something went wrong: {contextFeature.Error}");
						await context.Response.WriteAsync(new ErrorDetails()
						{
							StatusCode = context.Response.StatusCode,
							Message = "Internal Server Error."
						}.ToString());
					}
				});
			});


app.Run(async (context) =>
{
	int x = 0;
	int y = 8 / x;
	await context.Response.WriteAsync($"Result = {y}");
});

app.UseHttpsRedirection();
app.Run();

public class ErrorDetails
{
	public int StatusCode { get; set; }
	public string? Message { get; set; }
	public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

