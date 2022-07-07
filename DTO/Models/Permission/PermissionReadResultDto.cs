using System;
using System.Collections.Generic;
using Common.Models;
using Common.Models.Base;
using Domain.Enums;
using DTO.Models.Base;

namespace DTO.Models.Permission;

public class PermissionReadResultDto : IEntityBaseResultDto<Guid>, IDtoResultBase
{
    public string Alias { get; set; }
    public PermissionCompareMode CompareMode { get; set; }
    public PermissionType Type { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}