namespace SavageMetals.Api.Models;

public class SalesOrder
{
    public int Id { get; set; }
    public string SoNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Vendor Customer { get; set; } = null!;
    public string ContainerNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public AppUser CreatedByUser { get; set; } = null!;
    public List<SalesOrderLine> Lines { get; set; } = [];
}