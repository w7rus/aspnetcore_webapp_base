using Common.Enums;

namespace Common.Models;

public class FilterExpressionModelItemExpression : FilterExpressionModelItem
{
    public string Key { get; set; }
    public byte[] Value { get; set; }
    public FilterMatchOperation FilterMatchOperation { get; set; }
}