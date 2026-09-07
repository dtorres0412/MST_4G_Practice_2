namespace MST_4G.Dtos;

public record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)(PageSize > 0 ? PageSize : 1));
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}