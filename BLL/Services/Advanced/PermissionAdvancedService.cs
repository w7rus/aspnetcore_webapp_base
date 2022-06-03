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
    /// <param name="entityPermissionValueLeft">Equation left-side PermissionValue</param>
    /// <param name="entityPermissionValueRight">Equation right-side PermissionValue</param>
    /// <typeparam name="TEntityLeft">Type of left-side PermissionValue</typeparam>
    /// <typeparam name="TEntityRight">Type of right-side PermissionValue</typeparam>
    /// <returns>True=Authorized, False=Unauthorized</returns>
    bool Authorize<TEntityLeft, TEntityRight>(
        EntityPermissionValueBase<TEntityLeft> entityPermissionValueLeft,
        EntityPermissionValueBase<TEntityRight> entityPermissionValueRight
    ) where TEntityLeft : EntityBase<Guid> where TEntityRight : EntityBase<Guid>;

    /// <summary>
    /// Authorizes PermissionValue.Value to given custom value
    /// </summary>
    /// <param name="entityPermissionValueLeft">Equation left-side PermissionValue</param>
    /// <param name="_valueCompared">Equation right-side value</param>
    /// <typeparam name="TEntityLeft">Type of Compared PermissionValue</typeparam>
    /// <returns>True=Authorized, False=Unauthorized</returns>
    bool Authorize<TEntityLeft>(
        EntityPermissionValueBase<TEntityLeft> entityPermissionValueLeft,
        byte[] _valueCompared
    ) where TEntityLeft : EntityBase<Guid>;
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
        EntityPermissionValueBase<TEntity> entityPermissionValueLeft,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueRight
    ) where TEntity : EntityBase<Guid> where TEntityCompared : EntityBase<Guid>
    {
        if (entityPermissionValueLeft == null)
            return false;

        if (entityPermissionValueRight == null)
            return false;

        if (entityPermissionValueLeft.Permission.ValueType != entityPermissionValueRight.Permission.ValueType)
            throw new CustomException(
                $"{nameof(entityPermissionValueLeft.Permission.ValueType)} does not match {nameof(entityPermissionValueRight.Permission.ValueType)}");

        if (entityPermissionValueLeft.Permission.CompareMode != entityPermissionValueRight.Permission.CompareMode)
            throw new CustomException(
                $"{nameof(entityPermissionValueLeft.Permission.CompareMode)} does not match {nameof(entityPermissionValueRight.Permission.CompareMode)}");

        int compareToResult;

        switch (entityPermissionValueLeft.Permission.ValueType)
        {
            case PermissionValueType.Unknown:
                return false;
            case PermissionValueType.Boolean:
            {
                var value = BitConverter.ToBoolean(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToBoolean(entityPermissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int8:
            {
                var value = (sbyte) entityPermissionValueLeft.Value[0];
                var valueCompared = (sbyte) entityPermissionValueRight.Value[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int16:
            {
                var value = BitConverter.ToInt16(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt16(entityPermissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int32:
            {
                var value = BitConverter.ToInt32(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt32(entityPermissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int64:
            {
                var value = BitConverter.ToInt64(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt64(entityPermissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt8:
            {
                var value = entityPermissionValueLeft.Value[0];
                var valueCompared = entityPermissionValueRight.Value[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt16:
            {
                var value = BitConverter.ToUInt16(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt16(entityPermissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt32:
            {
                var value = BitConverter.ToUInt32(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt32(entityPermissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt64:
            {
                var value = BitConverter.ToUInt64(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt64(entityPermissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Float:
            {
                var value = BitConverter.ToSingle(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToSingle(entityPermissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Double:
            {
                var value = BitConverter.ToDouble(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToDouble(entityPermissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Decimal:
            {
                var value = new decimal(
                    BitConverter.ToInt32(entityPermissionValueLeft.Value),
                    BitConverter.ToInt32(entityPermissionValueLeft.Value, sizeof(int)),
                    BitConverter.ToInt32(entityPermissionValueLeft.Value, sizeof(int) * 2),
                    entityPermissionValueLeft.Value[15] == 0x80,
                    entityPermissionValueLeft.Value[14]
                );
                var valueCompared = new decimal(
                    BitConverter.ToInt32(entityPermissionValueRight.Value),
                    BitConverter.ToInt32(entityPermissionValueRight.Value, sizeof(int)),
                    BitConverter.ToInt32(entityPermissionValueRight.Value, sizeof(int) * 2),
                    entityPermissionValueRight.Value[15] == 0x80,
                    entityPermissionValueRight.Value[14]
                );
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.String:
            {
                var value = BitConverter.ToString(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToString(entityPermissionValueRight.Value);
                compareToResult = string.Compare(value, valueCompared, StringComparison.Ordinal);
                break;
            }
            case PermissionValueType.DateTime:
            {
                var value = DateTime.FromBinary(BitConverter.ToInt64(entityPermissionValueLeft.Value));
                var valueCompared = DateTime.FromBinary(BitConverter.ToInt64(entityPermissionValueRight.Value));
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return entityPermissionValueLeft.Permission.CompareMode switch
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
        EntityPermissionValueBase<TEntity> entityPermissionValueLeft,
        byte[] _valueCompared
    ) where TEntity : EntityBase<Guid>
    {
        if (entityPermissionValueLeft == null)
            return false;

        int compareToResult;

        switch (entityPermissionValueLeft.Permission.ValueType)
        {
            case PermissionValueType.Unknown:
                return false;
            case PermissionValueType.Boolean:
            {
                var value = BitConverter.ToBoolean(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToBoolean(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int8:
            {
                var value = (sbyte) entityPermissionValueLeft.Value[0];
                var valueCompared = (sbyte) _valueCompared[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int16:
            {
                var value = BitConverter.ToInt16(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt16(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int32:
            {
                var value = BitConverter.ToInt32(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt32(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int64:
            {
                var value = BitConverter.ToInt64(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt64(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt8:
            {
                var value = entityPermissionValueLeft.Value[0];
                var valueCompared = _valueCompared[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt16:
            {
                var value = BitConverter.ToUInt16(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt16(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt32:
            {
                var value = BitConverter.ToUInt32(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt32(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt64:
            {
                var value = BitConverter.ToUInt64(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt64(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Float:
            {
                var value = BitConverter.ToSingle(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToSingle(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Double:
            {
                var value = BitConverter.ToDouble(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToDouble(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Decimal:
            {
                var value = new decimal(
                    BitConverter.ToInt32(entityPermissionValueLeft.Value),
                    BitConverter.ToInt32(entityPermissionValueLeft.Value, sizeof(int)),
                    BitConverter.ToInt32(entityPermissionValueLeft.Value, sizeof(int) * 2),
                    entityPermissionValueLeft.Value[15] == 0x80,
                    entityPermissionValueLeft.Value[14]
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
                var value = BitConverter.ToString(entityPermissionValueLeft.Value);
                var valueCompared = BitConverter.ToString(_valueCompared);
                compareToResult = string.Compare(value, valueCompared, StringComparison.Ordinal);
                break;
            }
            case PermissionValueType.DateTime:
            {
                var value = DateTime.FromBinary(BitConverter.ToInt64(entityPermissionValueLeft.Value));
                var valueCompared = DateTime.FromBinary(BitConverter.ToInt64(_valueCompared));
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return entityPermissionValueLeft.Permission.CompareMode switch
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