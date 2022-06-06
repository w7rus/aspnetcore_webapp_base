using System.Collections.Generic;

namespace Common.Models;

public class FilterExpressionModelItemScope : FilterExpressionModelItem
{
    public List<FilterExpressionModelItem> Items { get; set; }
}