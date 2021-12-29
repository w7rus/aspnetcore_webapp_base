using System.Collections.Generic;

namespace Common.Models
{
    public class PageModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public static PageModel Max => new PageModel {Page = 1, PageSize = int.MaxValue};
        public static PageModel Single => new PageModel {Page = 1, PageSize = 1};
        public static PageModel Default => new PageModel {Page = 1, PageSize = 10};
    }

    public class PageModel<T> : PageModel
    {
        public int Total { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}