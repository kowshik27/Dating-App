using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IUserRepository
{
    void Update(User user);

    Task<bool> SaveAllAsync();

    Task<IEnumerable<User>> GetUsersAsync();

    Task<User?> GetUserByIdAsync(int Id);

    Task<User?> GetUserByUsernameAsync(string username);

    Task<IEnumerable<MemberDTO>> GetMembersAsync();

    Task<MemberDTO?> GetMemberByNameAsync(string username);
}
