using System;
using Domain.Enums;
using DTO.Models.Base;

namespace DTO.Models.Permission;

public class PermissionReadFSPCollectionItemResultDto : IEntityBaseResultDto<Guid>
{
    public string Alias { get; set; }
    public PermissionCompareMode CompareMode { get; set; }
    public PermissionType Type { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}