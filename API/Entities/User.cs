namespace API.Entities;

public class User
{
  // [Key] if don't use Id column as primary key
  public int Id { get; set; }

  public required string UserName {get; set;}

  public required byte[] PasswordHash {get; set;}

  public required byte[] PasswordSalt {get; set;}
}
