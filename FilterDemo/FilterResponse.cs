namespace FilterDemo;

public class FilterResponse<T>
{
    public FilterResponse()
    { }

    public FilterResponse
        (List<T> data)
    {
        Data = data;
        PageSize = data.Count;
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
        = 1;

    public int PageCount
        => PageSize > 0
            ? (int)Math.Ceiling((double)TotalCount / PageSize)
            : 0;

    public int PageSize { get; set; }

    public int? TotalCount { get; set; }
}