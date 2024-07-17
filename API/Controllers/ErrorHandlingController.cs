using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ErrorHandlingController(DataContext context): MyBaseController
{
    [HttpGet("auth")]
    public ActionResult<string> GetAuth(){
        return Unauthorized("Unauthorized: Please Login to continue");
    }

    [HttpGet("not-found")]
    public ActionResult<User> GetNotFound(){
        var thing = context.Users.Find(-1);
        if(thing==null) return NotFound("Requested Info not Found");
        return thing;
    }

    [HttpGet("server-error")]
    public ActionResult<User> GetServerError(){
        var thing = context.Users.Find(-1);
        if(thing==null) throw new Exception("Internal Server Error");
        return thing;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest(){
        return BadRequest("Invalid Request !! Check Request Again...");
    }
}
