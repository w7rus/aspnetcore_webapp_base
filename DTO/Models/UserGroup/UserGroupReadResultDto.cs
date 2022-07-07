using System;
using System.Collections.Generic;
using Common.Models;
using Common.Models.Base;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupReadResultDto : IEntityBaseResultDto<Guid>, IDtoResultBase
{
    public string Alias { get; set; }
    public string Description { get; set; }
    public long Priority { get; set; }
    public bool IsSystem { get; set; }
    public Guid UserId { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}