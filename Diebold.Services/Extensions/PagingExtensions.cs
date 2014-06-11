using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Diebold.Services.Extensions
{
    public static class PagingExtensions
    {
        //Page index (current page) starts at 0.
        public static Page<T> ToPage<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            var totalItems = query.Count();

            // double check for the current page. This is here because after you delete the last item from last page, 
            // last page number remains from view. So, the Skip step will not be correctly calculated.
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            totalPages = totalPages == 0 ? 1 : totalPages;

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            else if (pageIndex > totalPages)
            {
                pageIndex = totalPages;
            }

            query = query.Skip((pageIndex - 1) * pageSize);
            query = query.Take(pageSize);

            return new Page<T>(pageIndex, query, pageSize, totalItems, totalPages);
        }

        //Page index (current page) starts at 0.
        public static Page<T> ToPage<T>(this IList<T> list, int pageIndex, int pageSize, int rowCount)
        {
            var totalItems = rowCount;

            // double check for the current page. This is here because after you delete the last item from last page, 
            // last page number remains from view. So, the Skip step will not be correctly calculated.
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            totalPages = totalPages == 0 ? 1 : totalPages;

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            else if (pageIndex > totalPages)
            {
                pageIndex = totalPages;
            }

            return new Page<T>(pageIndex, list, pageSize, totalItems, totalPages);
        }
    }

    public class Page<T> : IEnumerable<T>
    {
        private readonly IList<T> _items;

        public IList<T> Items
        {
            get { return _items; }
        }

        public Page(int currentPage, IEnumerable<T> items, int itemsPerPage, int totalItems, int totalPages)
        {
            PageIndex = currentPage;
            _items = items.ToList();
            TotalItems = totalItems;
            PageSize = itemsPerPage;
            TotalPages = totalPages;
        }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItems { get; private set; }
        public int TotalPages { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
