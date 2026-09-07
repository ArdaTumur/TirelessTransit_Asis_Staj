using System.ComponentModel.DataAnnotations;

namespace MyAPI.DTOs;

public class LoginDto
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = "";

    [Required]
    [StringLength(20)]
    public string Password { get; set; } = "";
}