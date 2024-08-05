using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);

    Task<PagedList<MemberDTO>> GetAllUserLikes(LikesParams likesParams);

    Task<IEnumerable<int>> GetUserLikedProfileIds(int userId);

    void AddUserLike(UserLike like);

    void DeleteUserLike(UserLike like);

    Task<bool> SaveChanges();

}
