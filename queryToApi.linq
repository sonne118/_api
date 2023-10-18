var builder = WebApplication.CreateBuilder();

builder.Services.AddHttpClient<IJsonReader,JsonReader>();

var app = builder.Build();

app.UseMiddleware(typeof(JsonMiddleware));

app.Run();

public class JsonMiddleware
{
	DateTime _date = DateTime.Now;

	public JsonMiddleware(RequestDelegate next)
	{
	}

	public async Task Invoke(HttpContext context)
	{
		Root root = null;
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
		};
		
		var getService = context.RequestServices.GetService<IJsonReader>();
	    var  streamJson = await	getService.Get("https://myfakeapi.com/api/users/");
		
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

public interface IJsonReader
{ 
	 Task<Stream> Get(string url);	
}

public class JsonReader : IJsonReader
{
	readonly HttpClient _client;

	public JsonReader(HttpClient client)
	{
		_client = client;
	}

	public async Task<Stream> Get(string url) =>await _client.GetStreamAsync(url);
}



