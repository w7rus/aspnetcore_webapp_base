using System;
using Common.Models.Base;

namespace DTO.Models.Auth;

public class AuthRefreshResultDto : DTOResultBase
{
    public Guid UserId { get; set; }
    public string JsonWebToken { get; set; }
    public DateTimeOffset JsonWebTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; }
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
}