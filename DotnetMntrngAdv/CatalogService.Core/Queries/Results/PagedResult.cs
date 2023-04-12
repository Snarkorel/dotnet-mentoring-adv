namespace CatalogService.Core.Queries.Results
{
    public class PagedResult<T> : List<T>
    {
        public int CurrentPage { get; }

        public int PagesCount { get; }

        public int PageSize { get; }

        public int TotalCount { get; }

        public PagedResult(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            PagesCount = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        public static PagedResult<T> ToPagedResult(IQueryable<T> result, int pageNumber, int pageSize)
        {
            var count = result.Count();
            var items = result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<T>(items, count, pageNumber, pageSize);
        }
    }
}
