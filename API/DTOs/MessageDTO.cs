namespace API.DTOs;

public class MessageDTO
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    public required string SenderUsername { get; set; }

    public required string SenderPhotoUrl { get; set; }

    public int ReceiverId { get; set; }

    public required string ReceiverUsername { get; set; }

    public required string ReceiverPhotoUrl { get; set; }

    public required string Content { get; set; }

    public DateTime? MessageReadAt { get; set; }

    public DateTime MessageSent { get; set; }
}
