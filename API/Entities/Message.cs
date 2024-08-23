namespace API.Entities;

public class Message
{
    public int Id { get; set; }

    public required string SenderUsername { get; set; }

    public required string ReceiverUsername { get; set; }

    public required string Content { get; set; }

    public DateTime? MessageReadAt { get; set; }

    public DateTime MessageSent { get; set; } = DateTime.UtcNow;

    public bool SenderDeleted { get; set; }

    public bool ReceiverDeleted { get; set; }


    // Navigation Props
    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public User Sender { get; set; } = null!;

    public User Receiver { get; set; } = null!;
}
