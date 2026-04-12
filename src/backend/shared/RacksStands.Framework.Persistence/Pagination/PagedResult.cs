using Microsoft.EntityFrameworkCore;

namespace RacksStands.Framework.Persistence.Pagination;

/// <summary>
/// Represents a paginated result set with metadata.
/// </summary>
/// <typeparam name="T">Type of items in the result.</typeparam>
public record PagedResult<T>
{
    /// <summary>
    /// The items for the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>
    /// Current page number (1‑based).
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public long TotalCount { get; init; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// True if there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// True if there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    // Private constructor – use factory methods
    private PagedResult() { }

    /// <summary>
    /// Creates a paged result from an IQueryable source asynchronously.
    /// </summary>
    /// <param name="source">The IQueryable source.</param>
    /// <param name="pageNumber">1‑based page number.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task containing the PagedResult.</returns>
    public static async Task<PagedResult<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        // Get total count and items in parallel for efficiency
        var totalCountTask = source.CountAsync(cancellationToken);
        var itemsTask = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        await Task.WhenAll(totalCountTask, itemsTask);

        var totalCount = totalCountTask.Result;
        var items = itemsTask.Result;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    /// <summary>
    /// Creates a paged result with a projection (map items to a different type).
    /// </summary>
    /// <typeparam name="TResult">Target type after projection.</typeparam>
    /// <param name="selector">Projection function.</param>
    /// <returns>A new PagedResult with projected items.</returns>
    public PagedResult<TResult> Map<TResult>(Func<T, TResult> selector)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        return new PagedResult<TResult>
        {
            Items = Items.Select(selector).ToList(),
            PageNumber = PageNumber,
            PageSize = PageSize,
            TotalCount = TotalCount,
            TotalPages = TotalPages
        };
    }
}
