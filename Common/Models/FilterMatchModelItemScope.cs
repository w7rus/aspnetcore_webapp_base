using System.Collections.Generic;

namespace Common.Models;

public class FilterMatchModelItemScope : FilterMatchModelItem
{
    public List<FilterMatchModelItem> Items { get; set; }
}