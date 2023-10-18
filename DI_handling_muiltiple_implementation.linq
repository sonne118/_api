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
  <Namespace>Microsoft.AspNetCore.Identity</Namespace>
  <Namespace>Microsoft.AspNetCore.Mvc</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Security.Claims</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

var builder = WebApplication.CreateBuilder();


builder.Services.AddControllersWithViews().AddApplicationPart(typeof(HomeController).Assembly);

builder.Services.AddScoped<OneService>();
builder.Services.AddScoped<SecondService>();


builder.Services.AddScoped<StrategyService>(provider => (type) =>
{
	return type switch
	{
		SomeType.One => provider.GetRequiredService<OneService>(),
		SomeType.Second => provider.GetRequiredService<SecondService>(),
		_ => throw new ArgumentException(nameof(type))
	};
});


var app = builder.Build();
app.MapDefaultControllerRoute();
app.Run();

public delegate ISomeService StrategyService(SomeType type);
public enum SomeType
{
	One,
	Second
}


public class HomeController : Controller
{
	private readonly StrategyService _service;
	public HomeController(StrategyService service)
	{
		_service = service;
	}

	[HttpGet("One")]
	public ActionResult Index()
	{
		var oneService = _service.Invoke(SomeType.One);	
		return new ContentResult
		{
			Content = oneService.Do()
		};
	}

	[HttpGet("Second")]
	public ActionResult Index2()
	{
		var oneService = _service.Invoke(SomeType.Second);
		return new ContentResult
		{
			Content = oneService.Do()
		};
	}
}


public interface ISomeService
{
	string Do();
}

public class OneService : ISomeService
{
	public string Do()
	{
		return "It's a OneService";
	}
}

public class SecondService : ISomeService
{
	public string Do()
	{
		return "It's a SecondSevice";
	}
}