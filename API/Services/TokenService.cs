using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config, UserManager<User> userManager) : ITokenService
{
    public async Task<string> CreateToken(User user)
    {
        var TokenKey = config["TokenKey"] ?? throw new Exception("Cannot access token key from appsettings");
        if (TokenKey.Length < 64) throw new Exception("Token Key must be longer, Make it more than 64 chars !!");

        var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey));

        var Claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
        };

        var roles = await userManager.GetRolesAsync(user);

        Claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

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
