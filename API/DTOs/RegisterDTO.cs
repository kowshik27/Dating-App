using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDTO
{
    [MinLength(3)]
    [MaxLength(30)]
    public required string UserName { get; set; }

    [MinLength(4)]
    [MaxLength(30)]
    public required string Password { get; set; }

}
