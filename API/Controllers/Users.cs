using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;




namespace API.Controllers;

[ApiController]
[Route("/api/[controller]")]  // /api/users

public class UsersController(DataContext context) : ControllerBase
{

    [HttpGet]
    
    public async Task<ActionResult<IEnumerable<User>>> GetUsers(){
        var users = await context.Users.ToListAsync();
        return users;
    }

    [HttpGet("{id:int}")]
    
    public async Task<ActionResult<User>> GetUser(int id){
        var user = await context.Users.FindAsync(id);
        if(user==null) return NotFound();
        return user;
    }
}
