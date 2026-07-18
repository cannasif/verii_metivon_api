namespace verii_metivon_api.Core.Paging;

public sealed class PagedFilter
{
    public string Column { get; init; } = string.Empty;
    public string Operator { get; init; } = "Equals";
    public string Value { get; init; } = string.Empty;
}

public abstract class PagedQuery
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public string SortBy { get; init; } = "code";
    public string SortDirection { get; init; } = "asc";
    public IReadOnlyList<PagedFilter> Filters { get; init; } = [];
    public string FilterLogic { get; init; } = "and";

    public int NormalizedPageNumber => Math.Max(1, PageNumber);
    public int NormalizedPageSize => Math.Clamp(PageSize, 1, 100);
    public bool IsDescending => string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
    public bool UseOrFilterLogic => string.Equals(FilterLogic, "or", StringComparison.OrdinalIgnoreCase);
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount)
{
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
