using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IUserRepository
{
    void Update(User user);

    Task<bool> SaveAllAsync();

    Task<IEnumerable<User>> GetUsersAsync();

    Task<User?> GetUserByIdAsync(int Id);

    Task<User?> GetUserByUsernameAsync(string username);

    Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);

    Task<MemberDTO?> GetMemberByNameAsync(string username);
}
