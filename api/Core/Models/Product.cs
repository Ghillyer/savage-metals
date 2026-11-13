namespace SavageMetals.Api.Core.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? PurchasePrice { get; set; }
    public decimal? SalesPrice { get; set; }
    public bool IsScrap { get; set; }
    public bool IsActive { get; set; } = true;
}