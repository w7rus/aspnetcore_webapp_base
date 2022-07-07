using System;
using System.Collections.Generic;
using Common.Models;
using Common.Models.Base;
using DTO.Models.Base;

namespace DTO.Models.PermissionValue;

public class PermissionValueCreateResultDto : IEntityBaseResultDto<Guid>, IDtoResultBase
{
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}