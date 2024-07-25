using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("/api/[controller]")]  // /api/users

[Authorize]

// User === Members (But serializer, MemberDTO is used)
public class UsersController(IUserRepository userRepository, IMapper mapper,
 IPhotoService photoService) : MyBaseController
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
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {

        var user = await userRepository.GetUserByUsernameAsync(User.GetUserName());

        if (user == null) return BadRequest("Username not found in DB");

        mapper.Map(memberUpdateDTO, user);

        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to updated the user data");
    }

    [HttpPost("upload-photo")]

    public async Task<ActionResult<PhotoDTO>> uploadPhoto(IFormFile imgFile)
    {
        Console.WriteLine("\nHere\n\n\n", User.GetUserName());
        var user = await userRepository.GetUserByUsernameAsync(User.GetUserName());

        if (user == null) return BadRequest("Username not found in DB; Upload failed!!");

        var result = await photoService.AddPhotoAsync(imgFile);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        user.Photos.Add(photo);

        if (await userRepository.SaveAllAsync()) return
        CreatedAtAction(nameof(GetUser), new { username = user.UserName },
         mapper.Map<PhotoDTO>(photo));

        return BadRequest("Problem Uploading Photo");
    }

    [HttpPut("set-profile-photo/{photoId}")]
    public async Task<ActionResult> SetProfilePhoto(int photoId)
    {
        Console.WriteLine("In Profile Photo Method");
        var user = await userRepository.GetUserByUsernameAsync(User.GetUserName());
        if (user == null) return BadRequest("User doesn't exists !!");

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        Console.WriteLine("\n\n\nIam Here\n", photo);

        if (photo == null || photo.IsMain) return BadRequest("Cannot set the image as Main Photo");

        var CurrMainPhoto = user.Photos.FirstOrDefault(p => p.IsMain);

        if (CurrMainPhoto != null) CurrMainPhoto.IsMain = false;
        photo.IsMain = true;

        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Problem with updating Main Photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUserName());

        if (user == null) return BadRequest("User doesn't exists !!");

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("Cannot delete the photo");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem Deleting Photo");
    }

}
