using System;
using Domain.Enums;

namespace DTO.Models.Permission;

public class PermissionReadCollectionItemResult
{
    public Guid Id { get; set; }
    public string Alias { get; set; }
    public PermissionValueType ValueType { get; set; }
    public PermissionCompareMode CompareMode { get; set; }
    public PermissionType Type { get; set; }
}