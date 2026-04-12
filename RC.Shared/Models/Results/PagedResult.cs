using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Shared.Models.Results
{
    public class PagedResult<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRows { get; set; }
        public IEnumerable<T> Results { get; set; }
    }
}
