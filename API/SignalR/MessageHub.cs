using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessagesRepository messagesRepository,
 IUserRepository userRepository,
  IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine("\n\n Hi For MsgH");
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];

        if (Context.User == null || string.IsNullOrEmpty(otherUser))
            throw new Exception("Cannot create group for live messaging");

        var groupName = GetGroupName(Context.User.GetUserName(), otherUser!);

        Console.WriteLine("Hub Conn - ", groupName);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = messagesRepository.GetMessageThread(Context.User.GetUserName(), otherUser!);


        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDTO createMessageDTO)
    {
        var senderUsername = Context.User?.GetUserName()
            ?? throw new Exception("Cannot find the user");
        var receiverUsername = createMessageDTO.ReceiverUsername.ToLower();

        if (receiverUsername == senderUsername) throw new HubException("You cannot send Message yourself");

        var sender = await userRepository.GetUserByUsernameAsync(senderUsername);
        var receiver = await userRepository.GetUserByUsernameAsync(receiverUsername);

        if (sender?.UserName == null || receiver?.UserName == null) throw new HubException("Cannot Send Message Now");

        var message = new Message
        {
            Sender = sender,
            Receiver = receiver,
            SenderUsername = senderUsername,
            ReceiverUsername = receiverUsername,
            Content = createMessageDTO.Content,
        };

        // -------------- Code for MessageRead -----
        var groupName = GetGroupName(senderUsername, receiverUsername);
        var group = await messagesRepository.GetGroup(groupName);

        if (group != null &&
        group.Connections.Any(x => x.Username == receiverUsername))
        {
            message.MessageReadAt = DateTime.UtcNow;
        }
        // --------- Notify the Users about unread msg ----------------->
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUser(receiverUsername);

            if (connections != null && connections.Count != 0)
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                new { username = sender.UserName, nickname = sender.NickName });
            }
        }

        messagesRepository.AddMessage(message);

        if (await messagesRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
        }
    }

    private async Task<Group> AddToGroup(string grpName)
    {
        var username = Context.User?.GetUserName() ?? throw new Exception("UserName Not Found in AddToGroup Fx");
        var group = await messagesRepository.GetGroup(grpName);
        var connection = new Connection
        {
            ConnectionId = Context.ConnectionId,
            Username = username
        };

        if (group == null)
        {
            group = new Group { Name = grpName };
            messagesRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if (await messagesRepository.SaveAllAsync())
            return group;
        throw new HubException("Failed to join group");

    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await messagesRepository.GetGroupFromConnection(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

        if (connection != null && group != null)
        {
            messagesRepository.RemoveConnection(connection);
            if (await messagesRepository.SaveAllAsync()) return group;
        }
        throw new HubException("Failed to remove from group");
    }

    public string GetGroupName(string sender, string other)
    {
        var strCompare = string.CompareOrdinal(sender, other) < 0;
        Console.WriteLine("Groupname fx ->", strCompare);
        return strCompare ? $"{sender}-{other}" : $"{other}-{sender}";
    }
}
