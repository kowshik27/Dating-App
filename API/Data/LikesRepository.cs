using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public void AddUserLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteUserLike(UserLike like)
    {
        context.Likes.Remove(like);
    }


    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<IEnumerable<int>> GetUserLikedProfileIds(int userId)
    {
        return await context.Likes
        .Where(s => s.SourceUserId == userId)
        .Select(t => t.TargetUserId)
        .ToListAsync();
    }

    public async Task<PagedList<MemberDTO>> GetAllUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDTO> query;


        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
            .Where(s => s.SourceUserId == likesParams.UserId)
            .Select(t => t.TargetUser)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes
            .Where(t => t.TargetUserId == likesParams.UserId)
            .Select(s => s.SourceUser)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
            default:
                var likedIds = await GetUserLikedProfileIds(likesParams.UserId); // Gets User Liked Profile Ids
                query = likes
            .Where(x => x.TargetUserId == likesParams.UserId && likedIds.Contains(x.SourceUserId))
            .Select(s => s.SourceUser)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
        }

        return await PagedList<MemberDTO>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
