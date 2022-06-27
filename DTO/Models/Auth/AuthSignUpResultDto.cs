using System;
using Common.Models.Base;

namespace DTO.Models.Auth;

public class AuthSignUpResultDto : DTOResultBase
{
    public Guid UserId { get; set; }
}