using System;
using Common.Models;
using Common.Models.Base;
using Microsoft.AspNetCore.Mvc;

namespace DTO.Models.Auth
{
    public class AuthSignUpResult : DTOResultBase
    {
        /// <summary>
        /// Id of a user signed up
        /// </summary>
        public Guid UserId { get; set; }
    }
}