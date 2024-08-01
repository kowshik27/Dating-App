using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class AccountController(DataContext context,
 ITokenService tokenService, IMapper mapper) : MyBaseController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {

        if (await UserExists(registerDTO.Username)) return BadRequest("UserName already taken !!");

        using var hmac = new HMACSHA512();
        var user = mapper.Map<User>(registerDTO);

        user.UserName = registerDTO.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        user.PasswordSalt = hmac.Key;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var username = user.UserName;
        var nickName = user.NickName;
        var userId = user.Id;

        return new UserDTO
        {
            Username = username,
            NickName = nickName,
            Gender = user.Gender,
            Token = tokenService.CreateToken(username, userId),
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginData)
    {
        var user = await context.Users
        .Include(u => u.Photos)
        .FirstOrDefaultAsync(user => user.UserName == loginData.UserName.ToLower());
        if (user == null) return Unauthorized("Invalid UserName !!");

        var username = user.UserName;
        var passwordsalt = user.PasswordSalt;
        var passwordhash = user.PasswordHash;
        var nickName = user.NickName;
        var userId = user.Id;

        using var hmac = new HMACSHA512(passwordsalt);
        var HashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginData.Password));

        for (int i = 0; i < HashedPassword.Length; i++)
        {
            if (HashedPassword[i] != passwordhash[i]) return Unauthorized("Invalid Password, Enter Correct Password !!");
        }
        return new UserDTO
        {
            Username = username,
            Token = tokenService.CreateToken(username, userId),
            NickName = nickName,
            Gender = user.Gender,   
            ProfilePhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url
        };
        
    }

    private async Task<bool> UserExists(string UserName)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == UserName.ToLower());
    }
}
