using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Shared.Models
{
    public class PropertyPaginatedList<T>
    {
        public List<T> Items { get; set; }
        public string ProvinceCoordinates { set; get; }
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        public PropertyPaginatedList(List<T> items, string provinceCoordinates, int count, int pageIndex, int pageSize)
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
            ProvinceCoordinates = provinceCoordinates;
        }
        public PropertyPaginatedList(List<T> items, string provinceCoordinates, int count, int pageIndex, int pageSize, bool check)
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
            ProvinceCoordinates = provinceCoordinates;
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PropertyPaginatedList<T>> CreateAsync(IQueryable<T> source, string provinceCoordinates, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();

            if (count == 0)
            {
                return new PropertyPaginatedList<T>(new List<T>(),provinceCoordinates , count, 0, 1);
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

            return new PropertyPaginatedList<T>(items, provinceCoordinates, count, pageIndex, pageSize, true);
        }

        public static PropertyPaginatedList<T> Create(List<T> source, string provinceCoordinates, int pageIndex, int pageSize)
        {
            var count = source.Count;
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PropertyPaginatedList<T>(items, provinceCoordinates, count, pageIndex, pageSize, true);
        }
    }
}
