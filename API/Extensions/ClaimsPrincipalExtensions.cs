using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {

        var username = user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new Exception("Username not found in the Claims Principal");
        return username;

    }
}
