using System;
using System.ComponentModel.DataAnnotations;
using Common.Models;

namespace DTO.Models.Auth;

public class AuthSignInDto
{
    [Required]
    [RegularExpression(RegexExpressions.Email)]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    // /// <summary>
    // /// Token returned by client-side implementation of GoogleReCaptchaV2
    // /// </summary>
    // [GoogleReCaptchaV2]
    // public string RecaptchaToken { get; set; }
    public DateTimeOffset? RefreshTokenExpireAt { get; set; }
    public bool UseCookies { get; set; } = true;
}