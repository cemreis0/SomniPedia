namespace SomniPedia.Core.DTOs;

public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public PagedResponse(IEnumerable<T> data, int page, int pageSize, long totalCount)
    {
        Data = data;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
