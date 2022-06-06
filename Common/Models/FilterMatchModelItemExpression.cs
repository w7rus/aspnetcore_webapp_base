using Common.Enums;

namespace Common.Models;

public class FilterMatchModelItemExpression : FilterMatchModelItem
{
    public string Key { get; set; }
    public byte[] Value { get; set; }
    public FilterMatchOperation FilterMatchOperation { get; set; }
}