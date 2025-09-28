using System;
using System.Collections.Generic;
using System.Linq;

namespace NextParkAPI.Models.Responses
{
    public class PagedResponse<T>
    {
        public PagedResponse(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
            Links = new List<Link>();
        }

        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<Link> Links { get; set; }
    }
}
