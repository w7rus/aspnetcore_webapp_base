using System;
using System.Collections.Generic;
using Common.Models;
using Common.Models.Base;

namespace DTO.Models.Auth;

public class AuthSignUpResultDto : IDtoResultBase
{
    public Guid UserId { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}