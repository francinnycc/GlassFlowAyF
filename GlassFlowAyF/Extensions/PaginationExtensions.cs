using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Extensions
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();

        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public bool HasPreviousPage =>
            PageIndex > 1;

        public bool HasNextPage =>
            PageIndex < TotalPages;
    }

    public static class PaginationExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            int pageIndex,
            int pageSize)
        {
            if (pageIndex < 1)
                pageIndex = 1;

            if (pageSize < 1)
                pageSize = 10;

            var totalCount = await query.CountAsync();

            var totalPages =
                (int)Math.Ceiling(
                    totalCount / (double)pageSize);

            if (totalPages > 0 && pageIndex > totalPages)
                pageIndex = totalPages;

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                TotalCount = totalCount
            };
        }
    }
}