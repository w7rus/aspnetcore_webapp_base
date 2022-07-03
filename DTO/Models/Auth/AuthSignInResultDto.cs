using System;
using System.Collections.Generic;
using Common.Models;
using Common.Models.Base;

namespace DTO.Models.Auth;

public class AuthSignInResultDto : IDtoResultBase
{
    public Guid UserId { get; set; }
    public string JsonWebToken { get; set; }
    public DateTimeOffset JsonWebTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; }
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}