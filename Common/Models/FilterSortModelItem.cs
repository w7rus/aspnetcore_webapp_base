using Common.Enums;

namespace Common.Models;

public class FilterSortModelItem
{
    public string Key { get; set; }
    public FilterSortMode FilterSortMode { get; set; }
}