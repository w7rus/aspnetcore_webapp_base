using System;
using System.Text;
using Common.Exceptions;
using Common.Models;
using Domain.Entities;
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
    /// <param name="permissionValueLeft">Equation left-side PermissionValue</param>
    /// <param name="permissionValueRight">Equation right-side PermissionValue</param>
    /// <typeparam name="TEntityLeft">Type of left-side PermissionValue</typeparam>
    /// <typeparam name="TEntityRight">Type of right-side PermissionValue</typeparam>
    /// <returns>True=Authorized, False=Unauthorized</returns>
    bool Authorize(PermissionValue permissionValueLeft, PermissionValue permissionValueRight);

    /// <summary>
    /// Authorizes PermissionValue.Value to given custom value
    /// </summary>
    /// <param name="permissionValueLeft">Equation left-side PermissionValue</param>
    /// <param name="_valueCompared">Equation right-side value</param>
    /// <typeparam name="TEntityLeft">Type of Compared PermissionValue</typeparam>
    /// <returns>True=Authorized, False=Unauthorized</returns>
    bool Authorize(PermissionValue permissionValueLeft, byte[] _valueCompared);
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

    public bool Authorize(PermissionValue permissionValueLeft, PermissionValue permissionValueRight)
    {
        if (permissionValueLeft == null)
            return false;

        if (permissionValueRight == null)
            return false;

        if (permissionValueLeft.Permission.ValueType != permissionValueRight.Permission.ValueType)
            throw new CustomException(
                $"{nameof(permissionValueLeft.Permission.ValueType)} does not match {nameof(permissionValueRight.Permission.ValueType)}");

        if (permissionValueLeft.Permission.CompareMode != permissionValueRight.Permission.CompareMode)
            throw new CustomException(
                $"{nameof(permissionValueLeft.Permission.CompareMode)} does not match {nameof(permissionValueRight.Permission.CompareMode)}");

        int compareToResult;

        switch (permissionValueLeft.Permission.ValueType)
        {
            case PermissionValueType.Unknown:
                return false;
            case PermissionValueType.Boolean:
            {
                var value = BitConverter.ToBoolean(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToBoolean(permissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int8:
            {
                var value = (sbyte) permissionValueLeft.Value[0];
                var valueCompared = (sbyte) permissionValueRight.Value[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int16:
            {
                var value = BitConverter.ToInt16(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt16(permissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int32:
            {
                var value = BitConverter.ToInt32(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt32(permissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int64:
            {
                var value = BitConverter.ToInt64(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt64(permissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt8:
            {
                var value = permissionValueLeft.Value[0];
                var valueCompared = permissionValueRight.Value[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt16:
            {
                var value = BitConverter.ToUInt16(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt16(permissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt32:
            {
                var value = BitConverter.ToUInt32(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt32(permissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt64:
            {
                var value = BitConverter.ToUInt64(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt64(permissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Float:
            {
                var value = BitConverter.ToSingle(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToSingle(permissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Double:
            {
                var value = BitConverter.ToDouble(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToDouble(permissionValueRight.Value);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Decimal:
            {
                var value = new decimal(
                    BitConverter.ToInt32(permissionValueLeft.Value),
                    BitConverter.ToInt32(permissionValueLeft.Value, sizeof(int)),
                    BitConverter.ToInt32(permissionValueLeft.Value, sizeof(int) * 2),
                    permissionValueLeft.Value[15] == 0x80,
                    permissionValueLeft.Value[14]
                );
                var valueCompared = new decimal(
                    BitConverter.ToInt32(permissionValueRight.Value),
                    BitConverter.ToInt32(permissionValueRight.Value, sizeof(int)),
                    BitConverter.ToInt32(permissionValueRight.Value, sizeof(int) * 2),
                    permissionValueRight.Value[15] == 0x80,
                    permissionValueRight.Value[14]
                );
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.String:
            {
                var value = BitConverter.ToString(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToString(permissionValueRight.Value);
                compareToResult = string.Compare(value, valueCompared, StringComparison.Ordinal);
                break;
            }
            case PermissionValueType.DateTime:
            {
                var value = DateTime.FromBinary(BitConverter.ToInt64(permissionValueLeft.Value));
                var valueCompared = DateTime.FromBinary(BitConverter.ToInt64(permissionValueRight.Value));
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return permissionValueLeft.Permission.CompareMode switch
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

    public bool Authorize(PermissionValue permissionValueLeft, byte[] _valueCompared)
    {
        if (permissionValueLeft == null)
            return false;

        int compareToResult;

        switch (permissionValueLeft.Permission.ValueType)
        {
            case PermissionValueType.Unknown:
                throw new CustomException(Localize.Error.PermissionValueTypeUnknown);
            case PermissionValueType.Boolean:
            {
                var value = BitConverter.ToBoolean(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToBoolean(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int8:
            {
                var value = (sbyte) permissionValueLeft.Value[0];
                var valueCompared = (sbyte) _valueCompared[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int16:
            {
                var value = BitConverter.ToInt16(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt16(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int32:
            {
                var value = BitConverter.ToInt32(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt32(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Int64:
            {
                var value = BitConverter.ToInt64(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToInt64(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt8:
            {
                var value = permissionValueLeft.Value[0];
                var valueCompared = _valueCompared[0];
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt16:
            {
                var value = BitConverter.ToUInt16(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt16(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt32:
            {
                var value = BitConverter.ToUInt32(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt32(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.UInt64:
            {
                var value = BitConverter.ToUInt64(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToUInt64(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Float:
            {
                var value = BitConverter.ToSingle(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToSingle(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Double:
            {
                var value = BitConverter.ToDouble(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToDouble(_valueCompared);
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            case PermissionValueType.Decimal:
            {
                var value = new decimal(
                    BitConverter.ToInt32(permissionValueLeft.Value),
                    BitConverter.ToInt32(permissionValueLeft.Value, sizeof(int)),
                    BitConverter.ToInt32(permissionValueLeft.Value, sizeof(int) * 2),
                    permissionValueLeft.Value[15] == 0x80,
                    permissionValueLeft.Value[14]
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
                var value = Encoding.UTF8.GetString(permissionValueLeft.Value);
                var valueCompared = Encoding.UTF8.GetString(_valueCompared);
                compareToResult = string.Compare(value, valueCompared, StringComparison.Ordinal);
                break;
            }
            case PermissionValueType.DateTime:
            {
                var value = DateTime.FromBinary(BitConverter.ToInt64(permissionValueLeft.Value));
                var valueCompared = DateTime.FromBinary(BitConverter.ToInt64(_valueCompared));
                compareToResult = value.CompareTo(valueCompared);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return permissionValueLeft.Permission.CompareMode switch
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