using System;
using System.Reflection;
using ValueType = Common.Enums.ValueType;

namespace Common.Extensions;

public static class PropertyInfoExtensions
{
    public static ValueType GetValueType(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.PropertyType == typeof(bool))
        {
            return ValueType.Boolean;
        }
        if (propertyInfo.PropertyType == typeof(sbyte))
        {
            return ValueType.Int8;
        }
        if (propertyInfo.PropertyType == typeof(short))
        {
            return ValueType.Int16;
        }
        if (propertyInfo.PropertyType == typeof(int))
        {
            return ValueType.Int32;
        }
        if (propertyInfo.PropertyType == typeof(long))
        {
            return ValueType.Int64;
        }
        if (propertyInfo.PropertyType == typeof(sbyte))
        {
            return ValueType.UInt8;
        }
        if (propertyInfo.PropertyType == typeof(ushort))
        {
            return ValueType.UInt16;
        }
        if (propertyInfo.PropertyType == typeof(uint))
        {
            return ValueType.UInt32;
        }
        if (propertyInfo.PropertyType == typeof(ulong))
        {
            return ValueType.UInt64;
        }
        if (propertyInfo.PropertyType == typeof(float))
        {
            return ValueType.Float;
        }
        if (propertyInfo.PropertyType == typeof(double))
        {
            return ValueType.Double;
        }
        if (propertyInfo.PropertyType == typeof(decimal))
        {
            return ValueType.Decimal;
        }
        if (propertyInfo.PropertyType == typeof(string))
        {
            return ValueType.String;
        }
        if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTimeOffset))
        {
            return ValueType.DateTime;
        }
        if (propertyInfo.PropertyType == typeof(Guid))
        {
            return ValueType.Guid;
        }

        return propertyInfo.PropertyType.IsEnum ? ValueType.Int32 : ValueType.Unknown;
    }
}