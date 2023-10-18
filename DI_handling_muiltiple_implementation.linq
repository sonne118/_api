var builder = WebApplication.CreateBuilder();
builder.Services.AddControllersWithViews().AddApplicationPart(typeof(HomeController).Assembly);

builder.Services.AddScoped<OneService>();
builder.Services.AddScoped<SecondService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

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

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1")); //https://localhost:5001/swagger/Index.html
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