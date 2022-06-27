using System;

namespace DTO.Models.Auth;

public class AuthSignUpInAsGuestDto
{
    // /// <summary>
    // /// Token returned by client-side implementation of GoogleReCaptchaV2
    // /// </summary>
    // [GoogleReCaptchaV2]
    // public string RecaptchaToken { get; set; }
    public DateTimeOffset? RefreshTokenExpireAt { get; set; }
    public bool UseCookies { get; set; } = true;
}