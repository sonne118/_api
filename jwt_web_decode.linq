var builder = WebApplication.CreateBuilder();
builder.Services.Configure<JwtIssuerOptions>(options =>
{
	options.Issuer = "Server";
	options.Audience = "http://localhost:5000";
	options.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("12345678901234567890")), SecurityAlgorithms.HmacSha256);
});

builder.Services.AddControllersWithViews().AddApplicationPart(typeof(HomeController).Assembly);

var app = builder.Build();
app.MapDefaultControllerRoute();
app.Run();

public class HomeController : Controller
{
	readonly IOptions<JwtIssuerOptions> _options;
	public HomeController(IOptions<JwtIssuerOptions> options)
	{
		_options = options;
	}

	[HttpGet]
	public ActionResult Index()
	{
		return new ContentResult
		{
			Content =
$@"<html>
    <body>
    <form action=""Home/Jwt"" method=""post"">
        <button type=""submit"">Get Token</button>
    </form>
    </body>
</html>",
			ContentType = "text/html"
		};
	}

	[HttpPost]
	public ActionResult Jwt()
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
		
		var content =
		$@"
<html>
<body>
    <strong>Content</strong>: {token}
    <br/><br/>
    <strong>Encoded Token</strong>: {outputToken}

    <hr />
    Copy the encoded token here to get the content of the token back
    <form action=""/Home/DecodeJwt"" method=""post"">
        <input type=""text"" name=""token"" />
        <button type=""submit"">Decode Token</button>
    </form>
</body>
</html>";

		return new ContentResult
		{
			Content = content,
			ContentType = "text/html"
		};
	}

	[HttpPost]
	public ActionResult DecodeJwt([FromForm] string token)
	{
		var jwt = new JwtSecurityToken(token);

		var content = $@"<html>
<body>
    <strong>Content</strong>: {jwt}
</body>
</html>";
		return new ContentResult
		{
			Content = content,
			ContentType = "text/html"
		};
	}
}


public class JwtIssuerOptions
{
	public string Issuer { get; set; }

	public string Audience { get; set; }

	public SigningCredentials SigningCredentials { get; set; }
}

