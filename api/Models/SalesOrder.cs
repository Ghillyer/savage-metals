namespace SavageMetals.Api.Models;

public class SalesOrder
{
    public int Id { get; set; }
    public string SoNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string ContainerNumber { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public int CreatedByUserId { get; set; }
}