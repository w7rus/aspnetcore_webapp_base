using System;
using System.ComponentModel.DataAnnotations;

namespace DTO.Models.Auth;

public class AuthRefreshDto
{
    public string RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpireAt { get; set; }
}