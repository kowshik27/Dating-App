using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessagesRepository(DataContext context,
IMapper mapper) : IMessagesRepository
{


    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<PagedList<MessageDTO>> GetAllMessages(MessageParams messageParams)
    {
        var query = context.Messages
        .OrderByDescending(x => x.MessageSent)
        .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.ReceiverUsername == messageParams.Username && x.ReceiverDeleted == false),
            "Outbox" => query.Where(x => x.SenderUsername == messageParams.Username && x.SenderDeleted == false),
            _ => query.Where(x => x.ReceiverUsername == messageParams.Username && x.MessageReadAt == null && x.ReceiverDeleted == false)
        };

        var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);

        return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

    }

    public async Task<Connection?> GetConnection(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetGroup(string grpName)
    {
        return await context.Groups
        .Include(x => x.Connections)
        .FirstOrDefaultAsync(x => x.Name == grpName);
    }

    public async Task<Group?> GetGroupFromConnection(string connectionID)
    {
        return await context.Groups
        .Include(x => x.Connections)
        .Where(x => x.Connections.Any(x => x.ConnectionId == connectionID))
        .FirstOrDefaultAsync();
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string receiverUsername)
    {
        var query = await context.Messages
        .Where(x => x.SenderUsername == currentUsername && x.ReceiverUsername == receiverUsername
        || x.ReceiverUsername == currentUsername && x.SenderUsername == receiverUsername)
        // .OrderByDescending(x => x.MessageSent)
        .ProjectTo<MessageDTO>(mapper.ConfigurationProvider)
        .ToListAsync();

        var unreadMessages = query.Where(x =>
        x.ReceiverUsername == currentUsername && x.MessageReadAt == null).ToList();

        if (unreadMessages.Count() > 0)
        {
            unreadMessages.ForEach(x => x.MessageReadAt = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }

        return unreadMessages;

    }

    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
