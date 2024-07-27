using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDTO
{
    [MinLength(3)]
    [MaxLength(30)]
    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string NickName { get; set; } = "";

    [Required]
    public string City { get; set; } = "";

    [Required]
    public string Country { get; set; } = "";

    [Required]
    public string Gender { get; set; } = "";

    [Required]
    public string DateOfBirth { get; set; } = "";

    [MinLength(4)]
    [MaxLength(30)]
    [Required]
    public string Password { get; set; } = "";

}
