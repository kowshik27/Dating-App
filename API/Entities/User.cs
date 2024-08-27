using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class User : IdentityUser<int>
{
  // DOB, City, Country, Gender, NickName, Photo - required
  // Intro, Phto - optional
  // CreatedAt, LastActive -> Default values

  public required string Gender { get; set; }

  public required DateOnly DateOfBirth { get; set; }

  public required string NickName { get; set; }

  public required string City { get; set; }

  public required string Country { get; set; }

  public string? Introduction { get; set; }

  public string? Interests { get; set; }

  public string? LookingFor { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime LastActive { get; set; } = DateTime.UtcNow;

  public List<Photo> Photos { get; set; } = [];

  public List<UserLike> LikedByOtherUsers { get; set; } = [];

  public List<UserLike> LikedUsers { get; set; } = [];

  public List<Message> MessagesSent { get; set; } = [];

  public List<Message> MessagesReceived { get; set; } = [];

  public ICollection<AppUserRole> UserRoles { get; set; } = [];
}
