using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessagesRepository
{
    void AddMessage(Message message);

    void DeleteMessage(Message message);

    Task<Message?> GetMessage(int id);

    Task<PagedList<MessageDTO>> GetAllMessages(MessageParams messageParams);

    Task<IEnumerable<MessageDTO>> GetMessageThread(string senderUsername, string receiverUsername);

    Task<bool> SaveAllAsync();

    void AddGroup(Group group);

    void RemoveConnection(Connection connection);

    Task<Connection?> GetConnection(string connectionId);

    Task<Group?> GetGroup(string grpName);

    // Get group from connectionID
    Task<Group?> GetGroupFromConnection(string connectionID);

}
