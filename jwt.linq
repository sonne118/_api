<Query Kind="Program">
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
  <NuGetReference>System.IdentityModel.Tokens.Jwt</NuGetReference>
  <NuGetReference>System.Threading.Tasks.Dataflow</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Security.Claims</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.IdentityModel.Tokens.Jwt</Namespace>
  <Namespace>Microsoft.IdentityModel.Tokens</Namespace>
</Query>

internal class Program
{
	private static void Main()
	{		
		var input = "supermytextexample";
		var securityKey1 = new byte[input.Length * sizeof(char)];

		var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("12345678901234567890"));
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
	         {
		      new Claim( ClaimTypes.UserData,"IsValid", ClaimValueTypes.String, "(local)" ),
			  new Claim(ClaimTypes.Role, "Admin")
	         }),
			Issuer = "self",
			Audience = "https://www.website.com",
			Expires =  DateTime.Now.AddMinutes(60),	
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey1), SecurityAlgorithms.HmacSha256)
		};


		var jwtHandler = new JwtSecurityTokenHandler();
		SecurityToken jwt = jwtHandler.CreateToken(tokenDescriptor);
		string jsonCompactSerializedString = jwtHandler.WriteToken(jwt);
	
		string encodedPayload = jsonCompactSerializedString.Split('.')[1];
		string decodedPayload = Base64UrlDecode(encodedPayload);
		object jsonObject = JsonConvert.DeserializeObject(decodedPayload);
		string formattedPayload = JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
		
		jsonCompactSerializedString.Dump();	 
		formattedPayload.Dump();
		
	}

	
	public static string Base64UrlDecode(string value, Encoding encoding = null)
	{
		string urlDecodedValue = value.Replace('_', '/').Replace('-', '+');

		switch (value.Length % 4)
		{
			case 2:
				urlDecodedValue += "==";
				break;
			case 3:
				urlDecodedValue += "=";
				break;
		}

		return Encoding.ASCII.GetString(Convert.FromBase64String(urlDecodedValue));
	}

	//Microsoft.IdentityModel.Tokens(but not System.IdentityModel.Tokens)
	//System.IdentityModel.Tokens.Jwt
	//System.Security.Claims
}

//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoiSXNWYWxpZCIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTY5NTc0MjY2OSwiZXhwIjoxNjk1NzQ2MjY5LCJpYXQiOjE2OTU3NDI2NjksImlzcyI6InNlbGYiLCJhdWQiOiJodHRwczovL3d3dy53ZWJzaXRlLmNvbSJ9.UhC2nr17CzXdmS7SC0wM2bhwcK7GgzluX6sW9ZC684o
//{
//	"http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata": "IsValid",
//  "role": "Admin",
//  "nbf": 1695742669,
//  "exp": 1695746269,
//  "iat": 1695742669,
//  "iss": "self",
//  "aud": "https://www.website.com"
//}