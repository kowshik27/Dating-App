using System.ComponentModel.DataAnnotations.Schema;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(IMessagesRepository messagesRepository,
 IUserRepository userRepository, IMapper mapper) : MyBaseController
{
    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateNewMessage([FromBody] CreateMessageDTO createMessageDTO)
    {
        var senderUsername = User.GetUserName();
        var receiverUsername = createMessageDTO.ReceiverUsername.ToLower();
        if (receiverUsername == senderUsername) return BadRequest("You cannot send Message yourself");

        var sender = await userRepository.GetUserByUsernameAsync(senderUsername);
        var receiver = await userRepository.GetUserByUsernameAsync(receiverUsername);

        if (sender == null || receiver == null) return BadRequest("Cannot Send Message Now");

        var message = new Message
        {
            Sender = sender,
            Receiver = receiver,
            SenderUsername = senderUsername,
            ReceiverUsername = receiverUsername,
            Content = createMessageDTO.Content,
        };

        messagesRepository.AddMessage(message);

        if (await messagesRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDTO>(message));

        return BadRequest("Failed!! Cannot Send Message Now");
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetAllMessages([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUserName();
        messageParams.PageSize = 2;
        var messages = await messagesRepository.GetAllMessages(messageParams);
        Response.AddPaginationHeader(messages);

        Console.WriteLine(messages);
        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUserName();
        if (currentUsername == username) return BadRequest("Cannot message yourself for now");

        return Ok(await messagesRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUserName();

        var message = await messagesRepository.GetMessage(id);

        if (message == null) return BadRequest("Message Cannot be Deleted");

        if (username != message.SenderUsername && username != message.ReceiverUsername) return Forbid();

        if (message.SenderUsername == username) message.SenderDeleted = true;

        if (message.ReceiverUsername == username) message.ReceiverDeleted = true;

        if (message is { SenderDeleted: true, ReceiverDeleted: true })
        {
            messagesRepository.DeleteMessage(message);
        }

        if (await messagesRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem with deleting message");
    }
}
