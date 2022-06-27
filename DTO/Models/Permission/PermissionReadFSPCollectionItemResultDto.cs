using System;
using Domain.Enums;

namespace DTO.Models.Permission;

public class PermissionReadFSPCollectionItemResultDto
{
    public Guid Id { get; set; }
    public string Alias { get; set; }
    public PermissionCompareMode CompareMode { get; set; }
    public PermissionType Type { get; set; }
}