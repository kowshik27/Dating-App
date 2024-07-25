using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(string Username)
    {
        var TokenKey = config["TokenKey"] ?? throw new Exception("Cannot access token key from appsettings");
        if (TokenKey.Length < 64) throw new Exception("Token Key must be longer, Make it more than 64 chars !!");

        var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey));

        var Claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, Username)
        };

        var Creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512Signature);

        var TokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(Claims),
            Expires = DateTime.UtcNow.AddDays(5),
            SigningCredentials = Creds
        };

        var TokenHandler = new JwtSecurityTokenHandler();
        var token = TokenHandler.CreateToken(TokenDescriptor);


        return TokenHandler.WriteToken(token);
    }
}
