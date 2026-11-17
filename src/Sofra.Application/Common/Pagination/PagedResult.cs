namespace Sofra.Application.Common.Pagination
{
    public record PagedResult<TResult>(
        IReadOnlyCollection<TResult> Items,
        int PageNumber,
        int PageSize,
        int TotalCount
    );
}
