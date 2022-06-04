using System.Collections.Generic;
using Common.Enums;

namespace Common.Models;

public class FilterMatchModel
{
    public IEnumerable<FilterMatchModelItem> MatchRules { get; set; }
}