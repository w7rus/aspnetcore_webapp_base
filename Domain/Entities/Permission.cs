using System;
using Common.Attributes;
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
///     A Permission Entity. Referenced by multiple EntityPermissionValueBase.
/// </summary>
public class Permission : EntityBase<Guid>
{
    [AllowFilterExpression]
    [AllowFilterSort]
    public string Alias { get; set; }

    [AllowFilterExpression]
    [AllowFilterSort]
    public PermissionCompareMode CompareMode { get; set; }

    [AllowFilterExpression]
    [AllowFilterSort]
    public PermissionType Type { get; set; }
}