using System.Security.Cryptography;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed()
{
    public static async Task SeedUsers(UserManager<User> userManager, RoleManager<AppRole> roleManager)
    {

        if (await userManager.Users.AnyAsync())
        {
            Console.WriteLine("Users table is not empty!!");
            return;
        }

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var users = JsonSerializer.Deserialize<List<User>>(userData, options);

        if (users == null) return;

        var roles = new List<AppRole>
        {
            new() {Name="Member"},
            new() {Name="Moderator"},
            new AppRole{Name="Admin"},
            new() {Name="Tester"}
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }


        foreach (var user in users)
        {
            user.UserName = user.UserName!.ToLower();
            await userManager.CreateAsync(user, "0p9o8i&U");
            await userManager.AddToRoleAsync(user, "Member");
        }

        // Creating Admin User

        var admin = new User
        {
            UserName = "admin",
            NickName = "Admin",
            Gender = "",
            City = "",
            Country = "",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-40)
        };

        await userManager.CreateAsync(admin, "0p9o8i&U");
        await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
    }
}
