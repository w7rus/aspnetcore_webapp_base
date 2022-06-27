using System;

namespace DTO.Models.Auth;

public class AuthSignUpInAsGuestDto
{
    // /// <summary>
    // /// Token returned by client-side implementation of GoogleReCaptchaV2
    // /// </summary>
    // [GoogleReCaptchaV2]
    // public string RecaptchaToken { get; set; }
    
    /// <summary>
    /// Session lifetime
    /// </summary>
    public DateTimeOffset? RefreshTokenExpireAt { get; set; }

    /// <summary>
    /// Whether to use Cookies to return JsonWebToken and RefreshToken
    /// Set to false to use DTO for JsonWebToken and RefreshToken (Vulnerable to XSS)
    /// </summary>
    public bool UseCookies { get; set; } = true;
}