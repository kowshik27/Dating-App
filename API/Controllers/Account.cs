using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context): MyBaseController
{
    [HttpPost("register")]
    c

        // Console.WriteLine("The Registered is -");
        // Console.WriteLine(user);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    private async Task<bool> UserExists(string UserName){
       return await context.Users.AnyAsync(x=>x.UserName.ToLower()==UserName.ToLower());
    }
}
