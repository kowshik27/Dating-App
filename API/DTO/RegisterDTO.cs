using System.ComponentModel.DataAnnotations;

namespace API.DTO;

public class RegisterDTO
{
    [MinLength(3)]
    [MaxLength(30)]
    public required string UserName { get; set; }

    [MinLength(6)]
    [MaxLength(30)]
    public required string Password { get; set; }

}
