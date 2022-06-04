using System.Collections.Generic;
using Common.Enums;

namespace Common.Models;

public class FilterSortModel
{
    public IEnumerable<FilterSortModelItem> SortRules { get; set; }
}