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
    bool Authorize<TEntity, TEntityCompared>(
        Permission permission,
        EntityPermissionValueBase<TEntity> entityPermissionValue,
        Permission permissionCompared,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueCompared
    ) where TEntity : EntityBase<Guid> where TEntityCompared : EntityBase<Guid>;
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

    public bool Authorize<TEntity, TEntityCompared>(
        Permission permission,
        EntityPermissionValueBase<TEntity> entityPermissionValue,
        Permission permissionCompared,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueCompared
    ) where TEntity : EntityBase<Guid> where TEntityCompared : EntityBase<Guid>
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
                var value = BitConverter.ToBoolean(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToBoolean(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Int8:
            {
                var value = (sbyte) entityPermissionValue.Value[0];
                var valueCompared = (sbyte) entityPermissionValueCompared.Value[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Int16:
            {
                var value = BitConverter.ToInt16(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToInt16(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Int32:
            {
                var value = BitConverter.ToInt32(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToInt32(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Int64:
            {
                var value = BitConverter.ToInt64(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToInt64(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.UInt8:
            {
                var value = entityPermissionValue.Value[0];
                var valueCompared = entityPermissionValueCompared.Value[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.UInt16:
            {
                var value = BitConverter.ToUInt16(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToUInt16(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.UInt32:
            {
                var value = BitConverter.ToUInt32(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToUInt32(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.UInt64:
            {
                var value = BitConverter.ToUInt64(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToUInt64(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Float:
            {
                var value = BitConverter.ToSingle(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToSingle(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Double:
            {
                var value = BitConverter.ToDouble(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToDouble(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.Decimal:
            {
                var value = new decimal(
                    BitConverter.ToInt32(entityPermissionValue.Value),
                    BitConverter.ToInt32(entityPermissionValue.Value, sizeof(int)),
                    BitConverter.ToInt32(entityPermissionValue.Value, sizeof(int) * 2),
                    entityPermissionValue.Value[15] == 0x80,
                    entityPermissionValue.Value[14]
                    );
                var valueCompared = new decimal(
                    BitConverter.ToInt32(entityPermissionValueCompared.Value),
                    BitConverter.ToInt32(entityPermissionValueCompared.Value, sizeof(int)),
                    BitConverter.ToInt32(entityPermissionValueCompared.Value, sizeof(int) * 2),
                    entityPermissionValueCompared.Value[15] == 0x80,
                    entityPermissionValueCompared.Value[14]
                );
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionType.String:
            {
                var value = BitConverter.ToString(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToString(entityPermissionValueCompared.Value);
                compareToResult = string.Compare(value, valueCompared, StringComparison.Ordinal);
                break;
            }
            case PermissionType.DateTime:
            {
                var value = DateTime.FromBinary(BitConverter.ToInt64(entityPermissionValue.Value));
                var valueCompared = DateTime.FromBinary(BitConverter.ToInt64(entityPermissionValueCompared.Value));
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