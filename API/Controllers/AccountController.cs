using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class AccountController(UserManager<User> userManager,
 ITokenService tokenService, IMapper mapper) : MyBaseController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {

        if (await UserExists(registerDTO.Username)) return BadRequest("UserName already taken !!");

        var user = mapper.Map<User>(registerDTO);

        user.UserName = registerDTO.Username.ToLower();

        var result = await userManager.CreateAsync(user, registerDTO.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        var username = user.UserName;
        var nickName = user.NickName;
        var userId = user.Id;

        return new UserDTO
        {
            Username = username,
            NickName = nickName,
            Gender = user.Gender,
            Token = await tokenService.CreateToken(user),
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginData)
    {
        var user = await userManager.Users
        .Include(u => u.Photos)
        .FirstOrDefaultAsync(user => user.NormalizedUserName == loginData.UserName.ToUpper());
        if (user == null) return Unauthorized("Invalid UserName !!");

        var result = await userManager.CheckPasswordAsync(user, loginData.Password);

        if (!result) return Unauthorized("User unauthorized");

        var username = user.UserName;
        var nickName = user.NickName;
        var userId = user.Id;

        return new UserDTO
        {
            Username = username!,
            Token = await tokenService.CreateToken(user),
            NickName = nickName,
            Gender = user.Gender,
            ProfilePhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url
        };

    }

    private async Task<bool> UserExists(string UserName)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedUserName == UserName.ToUpper());
    }
}
