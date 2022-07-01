using System.ComponentModel.DataAnnotations;

namespace DTO.Models.Auth;

public class AuthSignOutDto
{
    public string RefreshToken { get; set; }
}