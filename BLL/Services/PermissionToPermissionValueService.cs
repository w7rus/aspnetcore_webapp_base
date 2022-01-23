using System;
using System.Collections.Generic;
using Common.Exceptions;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public interface IPermissionToPermissionValueService
{
    bool Authorize(
        Permission permission,
        EntityPermissionValueBase<EntityBase<Guid>> entityPermissionValue,
        Permission permissionCompared,
        EntityPermissionValueBase<EntityBase<Guid>> entityPermissionValueCompared
    );
}

public class PermissionToPermissionValueService : IPermissionToPermissionValueService
{
    #region Fields

    private readonly ILogger<PermissionToPermissionValueService> _logger;

    #endregion

    #region Ctor

    public PermissionToPermissionValueService(ILogger<PermissionToPermissionValueService> logger)
    {
        _logger = logger;
    }

    #endregion

    #region Methods

    public bool Authorize(
        Permission permission,
        EntityPermissionValueBase<EntityBase<Guid>> entityPermissionValue,
        Permission permissionCompared,
        EntityPermissionValueBase<EntityBase<Guid>> entityPermissionValueCompared
    )
    {
        if (permission.Type != permissionCompared.Type)
            throw new CustomException();
        
        if (permission.CompareMode != permissionCompared.CompareMode)
            throw new CustomException();

        int compareToResult;
        
        switch (permission.Type)
        {
            case PermissionType.Unknown:
                return false;
            case PermissionType.Boolean:
            {
                var value = Convert.ToBoolean(entityPermissionValue.Value);
                var valueCompared = Convert.ToBoolean(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Int8:
            {
                var value = Convert.ToSByte(entityPermissionValue.Value);
                var valueCompared = Convert.ToSByte(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Int16:
            {
                var value = Convert.ToInt16(entityPermissionValue.Value);
                var valueCompared = Convert.ToInt16(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Int32:
            {
                var value = Convert.ToInt32(entityPermissionValue.Value);
                var valueCompared = Convert.ToInt32(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Int64:
            {
                var value = Convert.ToInt64(entityPermissionValue.Value);
                var valueCompared = Convert.ToInt64(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.UInt8:
            {
                var value = Convert.ToByte(entityPermissionValue.Value);
                var valueCompared = Convert.ToByte(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.UInt16:
            {
                var value = Convert.ToUInt16(entityPermissionValue.Value);
                var valueCompared = Convert.ToUInt16(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.UInt32:
            {
                var value = Convert.ToUInt32(entityPermissionValue.Value);
                var valueCompared = Convert.ToUInt32(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.UInt64:
            {
                var value = Convert.ToUInt64(entityPermissionValue.Value);
                var valueCompared = Convert.ToUInt64(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Float:
            {
                var value = Convert.ToSingle(entityPermissionValue.Value);
                var valueCompared = Convert.ToSingle(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Double:
            {
                var value = Convert.ToDouble(entityPermissionValue.Value);
                var valueCompared = Convert.ToDouble(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Decimal:
            {
                var value = Convert.ToDecimal(entityPermissionValue.Value);
                var valueCompared = Convert.ToDecimal(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.String:
            {
                var value = Convert.ToString(entityPermissionValue.Value);
                var valueCompared = Convert.ToString(entityPermissionValueCompared.Value);
                compareToResult = string.Compare(value, valueCompared, StringComparison.Ordinal);
                break;
            }
            case PermissionType.DateTime:
            {
                var value = Convert.ToDateTime(entityPermissionValue.Value);
                var valueCompared = Convert.ToDateTime(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return permission.CompareMode switch
        {
            PermissionCompareMode.None => true,
            PermissionCompareMode.Equal => compareToResult == 0,
            PermissionCompareMode.NotEqual => compareToResult != 0,
            PermissionCompareMode.Less => compareToResult < 0,
            PermissionCompareMode.LessOrEqual => compareToResult <= 0,
            PermissionCompareMode.Greater => compareToResult > 0,
            PermissionCompareMode.GreaterOrEqual => compareToResult >= 0,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #endregion
}