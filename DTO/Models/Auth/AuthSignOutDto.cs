using System.ComponentModel.DataAnnotations;

namespace DTO.Models.Auth
{
    public class AuthSignOutDto
    {
        /// <summary>
        /// RefreshToken to use for refreshing auth. If null, retrieve from cookies.
        /// </summary>
        public string RefreshToken { get; set; }
    }
}