namespace FilterDemo;

public class FilterResponse<T>
{
    public FilterResponse
        (List<T> data)
    {
        Data = data;
        Page = 1;
        PageSize = data.Count;
        TotalCount = null;
    }

    public FilterResponse
        (List<T> data, int page, int pageSize, int? totalCount)
    {
        Data = data;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public List<T> Data { get; set; }

    public int Page { get; set; }

    public int PageCount
        => PageSize > 0
            ? (int)Math.Ceiling((double)TotalCount / PageSize)
            : 0;

    public int PageSize { get; set; }

    public int? TotalCount { get; set; }
}