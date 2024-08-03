using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Authorize]
public class LikesController(ILikesRepository likesRepository) : MyBaseController
{
    [HttpPost("{targetUserId:int}")]

    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();

        if (sourceUserId == targetUserId) return BadRequest("You can't like your profile");

        var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

        if (existingLike == null)
        {
            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            likesRepository.AddUserLike(like);
        }
        else
        {
            likesRepository.DeleteUserLike(existingLike);
        }

        if (await likesRepository.SaveChanges()) return Ok();

        return BadRequest("Problem with Updating the Like....");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetLikesInfo(string predicate)
    {
        int userId = User.GetUserId();
        if (predicate == "liked")
        {
            return Ok(await likesRepository.GetUserLikedProfiles(userId));
        }
        else if (predicate == "likedBy")
        {
            return Ok(await likesRepository.GetLikedByOtherUsers(userId));
        }
        else
        {
            return Ok(await likesRepository.GetMutualLikes(userId));
        }
    }

    [HttpGet("list")]

    public async Task<ActionResult<IEnumerable<int>>> GetUserLikedIds()
    {
        return Ok(await likesRepository.GetUserLikedProfileIds(User.GetUserId()));
    }
}
