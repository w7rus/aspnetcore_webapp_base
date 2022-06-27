using System.Collections.Generic;

namespace Common.Models;

public class FilterSortModel
{
    public IEnumerable<FilterSortModelItem> SortRules { get; set; }
}