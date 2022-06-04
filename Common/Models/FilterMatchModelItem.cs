using System;
using System.Reflection;
using Common.Enums;
using ValueType = Common.Enums.ValueType;

namespace Common.Models;

public class FilterMatchModelItem
{
    public string Key { get; set; }
    public byte[] Value { get; set; }
    public ValueType ValueType { get; set; }
    public FilterMatchMode FilterMatchMode { get; set; }
}