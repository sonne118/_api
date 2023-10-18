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