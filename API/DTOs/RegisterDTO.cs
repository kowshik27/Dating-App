using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDTO
{
    [MinLength(3)]
    [MaxLength(30)]
    [Required]
    public string Username { get; set; } = "";

    [MinLength(4)]
    [MaxLength(30)]
    [Required]
    public string Password { get; set; } = "";

}
