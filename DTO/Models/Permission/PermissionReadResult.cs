using System;
using Common.Models.Base;
using Domain.Enums;

namespace DTO.Models.Permission;

public class PermissionReadResult : DTOResultBase
{
    public Guid Id { get; set; }
    public string Alias { get; set; }
    public PermissionCompareMode CompareMode { get; set; }
    public PermissionType Type { get; set; }
}