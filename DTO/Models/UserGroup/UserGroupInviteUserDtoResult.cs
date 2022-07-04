using System;
using System.Collections.Generic;
using Common.Models;
using Common.Models.Base;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupInviteUserDtoResult : IEntityBaseResultDto<Guid>, IDtoResultBase
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}