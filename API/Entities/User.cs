namespace API.Entities;

public class User
{
  // [Key] if don't use Id column as primary key
  public int Id { get; set; }

  public required string UserName {get; set;}
}
