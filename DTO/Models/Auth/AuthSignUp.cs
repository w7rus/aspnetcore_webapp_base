using System.ComponentModel.DataAnnotations;
using Common.Attributes;
using Common.Models;

namespace DTO.Models.Auth
{
    public class AuthSignUp
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

        /// <summary>
        /// Name used when displaying user profile page and instead of emails when signin in
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Username { get; set; }
    }
}