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
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

var app = WebApplication.Create();
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
		log.LogDebug($"Request: {context.Request.Path}");
		context.Response.Headers.Add("Content-Type", "text/plain333");
		await context.Response.WriteAsync($"Middleware is singleton. Keep refreshing the page. You will see that the date does not change {_date}.");
	}
}
