namespace API.Entities;

public class Photo
{
    // Id, Url, IsMain, PublicId
    public int Id { get; set; }

    public required string Url { get; set; }

    public bool IsMain { get; set; }

    public string? PublicId { get; set; }

    // Navigation Properties
    public int UserId { get; set; }

    public User User { get; set; } = null!;
}
