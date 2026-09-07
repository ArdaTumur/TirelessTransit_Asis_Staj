using System.ComponentModel.DataAnnotations;

namespace MyAPI.DTOs;

public class RegisterDto
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$")]
    public string Username { get; set; } = "";

    [Required]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*-_=+?*]).{8,}$")]
    public string Password { get; set; } = "";

    [Required]
    [RegularExpression(@"^(User|Admin)$")]
    public string Role { get; set; } = "";
}