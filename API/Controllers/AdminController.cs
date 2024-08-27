using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class AdminController(UserManager<User> userManager) : MyBaseController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("user-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {

        var users = await userManager.Users
        .OrderBy(x => x.UserName)
        .Select(x => new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
        })
        .ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> UpdateUserRoles(string username, [FromQuery] string roles)
    {

        if (string.IsNullOrEmpty(roles)) return BadRequest("Roles cannot be Empty or NUll");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("User not found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Couldn't add to User Roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Couldn't remove from User Roles");

        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "PhotoModeratorRole")]
    [HttpGet("photo-moderators")]
    public ActionResult GetPhotoModerators()
    {
        return Ok("Admin or Moderator can view");
    }
}
