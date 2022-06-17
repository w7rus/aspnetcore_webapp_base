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
            case PermissionValueType.None:
            case PermissionValueType.Unknown:
                return false;
            case PermissionValueType.Boolean:
            {
                var value = BitConverter.ToBoolean(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToBoolean(permissionValueRight.Value);
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
            case PermissionValueType.None:
            case PermissionValueType.Unknown:
                throw new CustomException(Localize.Error.PermissionValueTypeUnknown);
            case PermissionValueType.Boolean:
            {
                var value = BitConverter.ToBoolean(permissionValueLeft.Value);
                var valueCompared = BitConverter.ToBoolean(_valueCompared);
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