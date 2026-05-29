namespace CleanCQRSPOC.Presentation.Models;

public class OrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}
