namespace CleanCQRSPOC.Presentation.Models;

public class ProductQueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; }
    public string? SortDir { get; set; } = "asc";
}
