using System;
using Domain.Enums;

namespace DTO.Models.Permission;

public class PermissionReadFSPCollectionItemResult
{
    public Guid Id { get; set; }
    public string Alias { get; set; }
    public PermissionCompareMode CompareMode { get; set; }
    public PermissionType Type { get; set; }
}