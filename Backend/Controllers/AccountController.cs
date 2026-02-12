using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using api.Models;
using api.dbactions;


namespace api.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private static string secretKey = "gH`fd3sTI'[0K2UUD$0zJ!z^Fa/Dar]q6%%6Bq$0r>d+f{#<n]"; // Don't actually store here in prod

    public static string GenerateJwTToken(string username)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username)
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt_token = tokenHandler.WriteToken(token);
        return jwt_token;
    }

    public static ClaimsPrincipal? ValidateJWTToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        SecurityToken validatedToken;
        try
        {
            return tokenHandler.ValidateToken(token, validationParams, out validatedToken);
        } catch (Exception e)
        {
            Console.WriteLine($"There was an error at AccountController.ValidateToken()\n{e}");
            return null;
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserCredentials credentials)
    {
        if(!UsernameValidator.IsValid(credentials.Name)) return BadRequest("Illegal characters in username (a-z, A-Z, 0-9, _)!");
        if(!AccountManagement.UserAccountLogin(credentials.Name, credentials.Password)) return Unauthorized("Incorrect Credentials.");
        var token = GenerateJwTToken(credentials.Name);
        return Ok(new
        {
            accountName = credentials.Name,
            refreshToken = "ToBeImplemented",
            accessToken = token,
            tokenType = "Bearer",
            expiresIn = 1800
        });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] UserCredentials credentials)
    {
        if(!UsernameValidator.IsValid(credentials.Name)) return BadRequest("Illegal characters in username (a-z, A-Z, 0-9, _)!");
        if(!AccountManagement.CreateUserAccount(credentials.Name, credentials.Password)) return BadRequest("User already exists!");
        return Ok(new {status = "Success"});
    }

    [HttpGet("me")]
    public IActionResult TestAuth([FromHeader(Name = "Authorization")] string? authorization) // [FromForm] ToDoItem item
    {
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
        return Unauthorized();
        var token = authorization["Bearer ".Length..];
        var principal = AccountController.ValidateJWTToken(token);
        if(principal == null) return Unauthorized();
        var username = principal.Identity!.Name;
        return Ok(new
        {
            status = "Success",
            username = username
        });
    }
}