using System;
using System.ComponentModel.DataAnnotations;

namespace DTO.Models.Auth
{
    public class AuthRefresh
    {
        /// <summary>
        /// RefreshToken to use for refreshing auth. If null, retrieve from cookies.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Session lifetime
        /// </summary>
        public DateTimeOffset? RefreshTokenExpireAt { get; set; }
    }
}