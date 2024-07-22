using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("/api/[controller]")]  // /api/users

[Authorize]

// User === Members (But serializer, MemberDTO is used)
public class UsersController(IUserRepository userRepository, IMapper mapper) : MyBaseController
{


    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var members = await userRepository.GetMembersAsync();
        return Ok(members);
    }


    [HttpGet("{username}")]
    public async Task<ActionResult<User>> GetUser(string username)
    {
        var member = await userRepository.GetMemberByNameAsync(username);
        if (member == null) return NotFound();
        return Ok(member);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO){

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(username==null) return BadRequest("No username found in token");

        var user = await userRepository.GetUserByUsernameAsync(username);

        if(user==null) return BadRequest("Username not found in DB");

        mapper.Map(memberUpdateDTO, user);

        if(await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to updated the user data");
    }
}
