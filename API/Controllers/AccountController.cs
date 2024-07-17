using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class AccountController(DataContext context, ITokenService tokenService): MyBaseController
{   
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){

        if(await UserExists(registerDTO.Username)) return BadRequest("UserName already taken !!");

        // using var hmac = new HMACSHA512();
        // var user = new User{
        //     UserName = registerDTO.Username.ToLower(),
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
        //     PasswordSalt = hmac.Key
        // };

        // // Console.WriteLine("The Registered is -");
        // // Console.WriteLine(user);

        // context.Users.Add(user);
        // await context.SaveChangesAsync(); 
        
        // var username = user.UserName;

        // return new UserDTO{
        //     Username = username,
        //     Token = tokenService.CreateToken(username),
        // };

        return Ok(); // Remove it later
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginData){
        var user = await context.Users.FirstOrDefaultAsync(user => user.UserName == loginData.UserName.ToLower());
        if(user == null) return Unauthorized("Invalid UserName !!");
        
        var username = user.UserName;
        var passwordsalt = user.PasswordSalt;
        var passwordhash = user.PasswordHash;


        using var hmac = new HMACSHA512(passwordsalt);
        var HashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginData.Password));

        for (int i = 0; i < HashedPassword.Length; i++)
        {
            if(HashedPassword[i]!=passwordhash[i]) return Unauthorized("Invalid Password, Enter Correct Password !!");
        }
        return new UserDTO{
            Username = username,
            Token = tokenService.CreateToken(username),
        };
    }

    private async Task<bool> UserExists(string UserName){
       return await context.Users.AnyAsync(x=>x.UserName.ToLower()==UserName.ToLower());
    }
}
