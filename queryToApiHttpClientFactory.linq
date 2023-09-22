<Query Kind="Statements">
  <Output>DataGrids</Output>
  <NuGetReference>App.Metrics.AspNetCore</NuGetReference>
  <NuGetReference>App.Metrics.AspNetCore.Core</NuGetReference>
  <NuGetReference>Microsoft.AspNetCore.Http</NuGetReference>
  <NuGetReference>Microsoft.AspNetCore.Http.Features</NuGetReference>
  <NuGetReference>Microsoft.AspNetCore.Mvc</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Http.Polly</NuGetReference>
  <NuGetReference>NUnitLite</NuGetReference>
  <NuGetReference>System.Threading.Tasks.Dataflow</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

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