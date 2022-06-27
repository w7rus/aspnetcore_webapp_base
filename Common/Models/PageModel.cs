using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Models.Base;

namespace Common.Models;

public class PageModel
{
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    public static PageModel Max => new()
        {Page = 1, PageSize = int.MaxValue};

    public static PageModel Single => new()
        {Page = 1, PageSize = 1};

    public static PageModel Default => new()
        {Page = 1, PageSize = 10};
}

public class PageModelResult<T> : DTOResultBase
{
    public int Total { get; set; }
    public IEnumerable<T> Items { get; set; }
}