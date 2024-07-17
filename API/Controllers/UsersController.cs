using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/api/[controller]")]  // /api/users

[Authorize]

// User === Members (But serializer, MemberDTO is used)
public class UsersController(IUserRepository userRepository) : MyBaseController
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
}
