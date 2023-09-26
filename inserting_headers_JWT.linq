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
  <Namespace>System.Security.Claims</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

var builder = WebApplication.Create();


builder.Services.Configure<JwtIssuerOptions>(options =>
{
	options.Issuer = "Server";
	options.Audience = "http://localhost:5000";
	options.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("12345678901234567890")), SecurityAlgorithms.HmacSha256);
});
var app = builder.Build();
app.MapDefaultControllerRoute();
app.Run()

app.UseMiddleware(typeof(TerminalMiddleware));
app.Run();


builder.Services.AddControllersWithViews().AddApplicationPart(typeof(HomeController).Assembly);

var app = builder.Build();
app.MapDefaultControllerRoute();
app.Run();


public class TerminalMiddleware
{
	DateTime _date = DateTime.Now;

	public TerminalMiddleware(RequestDelegate next)
	{
	}

	public async Task Invoke(HttpContext context, ILogger<TerminalMiddleware> log)
	{
		var claims = new[]
	{
			  new Claim( ClaimTypes.UserData,"IsValid", ClaimValueTypes.String, "(local)" ),
			  new Claim(ClaimTypes.Role, "Admin")
			};

		var option = _options.Value;


		var input = "supermytextexample";
		var securityKey = new byte[input.Length * sizeof(char)];

		var token = new JwtSecurityToken(
		   issuer: "myServer",
		   audience: "http://localhost:5000",
		   claims: claims,
		   expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
		   signingCredentials: new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256));


		var outputToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

		log.LogDebug($"Request: {context.Request.Path}");
		context.Response.Headers.Add("Content-Type", "text/plain333");
		context.Response.Headers.Add("Authorization", "Bearer " + YourToken);
		await context.Response.WriteAsync($"Middleware is singleton. {_date}.");
	}

	
}

public class JwtIssuerOptions
{
	public string Issuer { get; set; }

	public string Audience { get; set; }

	public SigningCredentials SigningCredentials { get; set; }
}
