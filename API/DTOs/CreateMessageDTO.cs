namespace API.DTOs;

public class CreateMessageDTO
{
    public required string ReceiverUsername { get; set; }

    public required string Content { get; set; }
}
