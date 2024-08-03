using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);

    Task<IEnumerable<MemberDTO>> GetUserLikedProfiles(int userId);

    Task<IEnumerable<MemberDTO>> GetLikedByOtherUsers(int userId);

    Task<IEnumerable<MemberDTO>> GetMutualLikes(int userId);

    Task<IEnumerable<int>> GetUserLikedProfileIds(int userId);

    void AddUserLike(UserLike like);

    void DeleteUserLike(UserLike like);

    Task<bool> SaveChanges();

}
