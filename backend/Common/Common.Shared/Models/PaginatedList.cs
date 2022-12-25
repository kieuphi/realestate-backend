
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Shared.Models
{
    public class PaginatedList<T> 
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            if (count == 0)
            {
                TotalPages = 0;
            }
            else
            {
                TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            }
            
            TotalCount = count;
            Items = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize,bool check)
        {
            PageIndex = pageIndex;
            if (count == 0)
            {
                TotalPages = 0;
            }
            else
            {
                TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            }

            TotalCount = count;
            Items = items;
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();

            if (count == 0)
            {
                return new PaginatedList<T>(new List<T>(), count, 0, 1);
            }
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            if (pageSize == 0)
            {
                pageSize = count;
            }

            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize, true);
        }

        public static PaginatedList<T> Create(List<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count;
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<T>(items, count, pageIndex, pageSize, true);
        }
    }
}
