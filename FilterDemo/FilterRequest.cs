namespace FilterDemo;

public class FilterRequest
{
    public bool Count { get; set; }
    public List<CustomFilterExpression> CustomFilters { get; set; }
    public List<FilterExpression> Filters { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public List<string> Select { get; set; }
    public SortExpression Sort { get; set; }
}