using System.Collections.Generic;

namespace Common.Models;

public class FilterExpressionModelItemStackItem
{
    public List<FilterExpressionModelItem> Items { get; set; }
    public int Index { get; set; }
}