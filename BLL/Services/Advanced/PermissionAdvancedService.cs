using System;
using Common.Exceptions;
using Domain.Entities.Base;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Advanced;

/// <summary>
/// Advanced Service to authorize actions based on comparison of PermissionValues
/// </summary>
public interface IPermissionAdvancedService
{
    /// <summary>
    /// Authorizes PermissionValue.Value to another PermissionValue.Value
    /// </summary>
    /// <param name="entityPermissionValue">PermissionValue compared</param>
    /// <param name="entityPermissionValueCompared">Comparable PermissionValue</param>
    /// <typeparam name="TEntity">Type of Compared PermissionValue</typeparam>
    /// <typeparam name="TEntityCompared">Type of Comparable PermissionValue</typeparam>
    /// <returns>True=Authorized, False=Unauthorized</returns>
    bool Authorize<TEntity, TEntityCompared>(
        EntityPermissionValueBase<TEntity> entityPermissionValue,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueCompared
    ) where TEntity : EntityBase<Guid> where TEntityCompared : EntityBase<Guid>;

    /// <summary>
    /// Authorizes PermissionValue.Value to given custom value
    /// </summary>
    /// <param name="entityPermissionValue">PermissionValue compared</param>
    /// <param name="_valueCompared">Comparable value</param>
    /// <typeparam name="TEntity">Type of Compared PermissionValue</typeparam>
    /// <returns>True=Authorized, False=Unauthorized</returns>
    bool Authorize<TEntity>(
        EntityPermissionValueBase<TEntity> entityPermissionValue,
        byte[] _valueCompared
    ) where TEntity : EntityBase<Guid>;
}

public class PermissionAdvancedService : IPermissionAdvancedService
{
    #region Fields

    private readonly ILogger<PermissionAdvancedService> _logger;

    #endregion

    #region Ctor

    public PermissionAdvancedService(ILogger<PermissionAdvancedService> logger)
    {
        _logger = logger;
    }

    #endregion

    #region Methods

    public bool Authorize<TEntity, TEntityCompared>(
        EntityPermissionValueBase<TEntity> entityPermissionValue,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueCompared
    ) where TEntity : EntityBase<Guid> where TEntityCompared : EntityBase<Guid>
    {
        if (entityPermissionValue == null)
            return false;

        if (entityPermissionValueCompared == null)
            return false;

        if (entityPermissionValue.Permission.ValueType != entityPermissionValueCompared.Permission.ValueType)
            throw new CustomException(
                $"{nameof(entityPermissionValue.Permission.ValueType)} does not match {nameof(entityPermissionValueCompared.Permission.ValueType)}");

        if (entityPermissionValue.Permission.CompareMode != entityPermissionValueCompared.Permission.CompareMode)
            throw new CustomException(
                $"{nameof(entityPermissionValue.Permission.CompareMode)} does not match {nameof(entityPermissionValueCompared.Permission.CompareMode)}");

        int compareToResult;

        switch (entityPermissionValue.Permission.ValueType)
        {
            case PermissionValueType.Unknown:
                return false;
            case PermissionValueType.Boolean:
            {
                var value = BitConverter.ToBoolean(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToBoolean(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int8:
            {
                var value = (sbyte) entityPermissionValue.Value[0];
                var valueCompared = (sbyte) entityPermissionValueCompared.Value[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int16:
            {
                var value = BitConverter.ToInt16(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToInt16(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int32:
            {
                var value = BitConverter.ToInt32(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToInt32(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int64:
            {
                var value = BitConverter.ToInt64(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToInt64(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt8:
            {
                var value = entityPermissionValue.Value[0];
                var valueCompared = entityPermissionValueCompared.Value[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt16:
            {
                var value = BitConverter.ToUInt16(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToUInt16(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt32:
            {
                var value = BitConverter.ToUInt32(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToUInt32(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt64:
            {
                var value = BitConverter.ToUInt64(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToUInt64(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Float:
            {
                var value = BitConverter.ToSingle(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToSingle(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Double:
            {
                var value = BitConverter.ToDouble(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToDouble(entityPermissionValueCompared.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Decimal:
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
            case PermissionValueType.String:
            {
                var value = BitConverter.ToString(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToString(entityPermissionValueCompared.Value);
                compareToResult = string.Compare(value, valueCompared, StringComparison.Ordinal);
                break;
            }
            case PermissionValueType.DateTime:
            {
                var value = DateTime.FromBinary(BitConverter.ToInt64(entityPermissionValue.Value));
                var valueCompared = DateTime.FromBinary(BitConverter.ToInt64(entityPermissionValueCompared.Value));
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return entityPermissionValue.Permission.CompareMode switch
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

    public bool Authorize<TEntity>(
        EntityPermissionValueBase<TEntity> entityPermissionValue,
        byte[] _valueCompared
    ) where TEntity : EntityBase<Guid>
    {
        if (entityPermissionValue == null)
            return false;

        int compareToResult;

        switch (entityPermissionValue.Permission.ValueType)
        {
            case PermissionValueType.Unknown:
                return false;
            case PermissionValueType.Boolean:
            {
                var value = BitConverter.ToBoolean(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToBoolean(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int8:
            {
                var value = (sbyte) entityPermissionValue.Value[0];
                var valueCompared = (sbyte) _valueCompared[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int16:
            {
                var value = BitConverter.ToInt16(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToInt16(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int32:
            {
                var value = BitConverter.ToInt32(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToInt32(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int64:
            {
                var value = BitConverter.ToInt64(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToInt64(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt8:
            {
                var value = entityPermissionValue.Value[0];
                var valueCompared = _valueCompared[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt16:
            {
                var value = BitConverter.ToUInt16(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToUInt16(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt32:
            {
                var value = BitConverter.ToUInt32(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToUInt32(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt64:
            {
                var value = BitConverter.ToUInt64(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToUInt64(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Float:
            {
                var value = BitConverter.ToSingle(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToSingle(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Double:
            {
                var value = BitConverter.ToDouble(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToDouble(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Decimal:
            {
                var value = new decimal(
                    BitConverter.ToInt32(entityPermissionValue.Value),
                    BitConverter.ToInt32(entityPermissionValue.Value, sizeof(int)),
                    BitConverter.ToInt32(entityPermissionValue.Value, sizeof(int) * 2),
                    entityPermissionValue.Value[15] == 0x80,
                    entityPermissionValue.Value[14]
                );
                var valueCompared = new decimal(
                    BitConverter.ToInt32(_valueCompared),
                    BitConverter.ToInt32(_valueCompared, sizeof(int)),
                    BitConverter.ToInt32(_valueCompared, sizeof(int) * 2),
                    _valueCompared[15] == 0x80,
                    _valueCompared[14]
                );
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.String:
            {
                var value = BitConverter.ToString(entityPermissionValue.Value);
                var valueCompared = BitConverter.ToString(_valueCompared);
                compareToResult = string.Compare(value, valueCompared, StringComparison.Ordinal);
                break;
            }
            case PermissionValueType.DateTime:
            {
                var value = DateTime.FromBinary(BitConverter.ToInt64(entityPermissionValue.Value));
                var valueCompared = DateTime.FromBinary(BitConverter.ToInt64(_valueCompared));
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return entityPermissionValue.Permission.CompareMode switch
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