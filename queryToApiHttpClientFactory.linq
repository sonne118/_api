var builder = WebApplication.CreateBuilder();

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseMiddleware(typeof(TerminalMiddleware));

app.Run();

public class TerminalMiddleware
{
	DateTime _date = DateTime.Now;

	public TerminalMiddleware(RequestDelegate next)
	{
	}

	public async Task Invoke(HttpContext context, ILogger<TerminalMiddleware> log)
	{
		Root root = null;
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
		};

		var getService = context.RequestServices.GetRequiredService<IHttpClientFactory>();
		var httpClient = getService.CreateClient();

		var streamJson = await httpClient.GetStreamAsync("https://myfakeapi.com/api/users/");

		if (streamJson.CanRead)
		root = await System.Text.Json.JsonSerializer.DeserializeAsync<Root>(streamJson, options);

		var json = System.Text.Json.JsonSerializer.Serialize(root ?? new Root());
		await context.Response.WriteAsync(json);
	}
}

public class Root
{
	public List<User> Users { get; set; }
}

public class User
{
	[JsonPropertyName("id")]
	public int id { get; set; }

	[JsonPropertyName("first_name")]
	public string first_name { get; set; }

	[JsonPropertyName("last_name")]
	public string last_name { get; set; }

}