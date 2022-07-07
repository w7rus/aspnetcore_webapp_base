using System;
using System.Collections.Generic;
using Common.Models;
using Common.Models.Base;
using DTO.Models.Base;

namespace DTO.Models.User;

public class UserUpdateResultDto : IEntityBaseResultDto<Guid>, IDtoResultBase
{
    public string Email { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTimeOffset LastSignIn { get; set; }
    public DateTimeOffset LastActivity { get; set; }
    public string LastIpAddress { get; set; }
    public int FailedSignInAttempts { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}