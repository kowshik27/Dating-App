using API.DTOs;
using API.Entities;
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

    public async Task<IEnumerable<MemberDTO>> GetLikedByOtherUsers(int userId)
    {
        var likes = context.Likes.AsQueryable();

        return await likes
        .Where(t => t.TargetUserId == userId)
        .Select(s => s.SourceUser)
        .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<IEnumerable<MemberDTO>> GetMutualLikes(int userId)
    {
        var likes = context.Likes.AsQueryable();
        var likedIds = await GetUserLikedProfileIds(userId); // Gets User Liked Profile Ids

        return await likes
        .Where(x => x.TargetUserId == userId && likedIds.Contains(x.SourceUserId))
        .Select(s => s.SourceUser)
        .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
        .ToListAsync();
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

    public async Task<IEnumerable<MemberDTO>> GetUserLikedProfiles(int userId)
    {
        var likes = context.Likes.AsQueryable();

        return await likes
        .Where(s => s.SourceUserId == userId)
        .Select(t => t.TargetUser)
        .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
