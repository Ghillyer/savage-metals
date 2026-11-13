using SavageMetals.Api.Enums;

namespace SavageMetals.Api.Models;

public class Vendor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public VendorType Type { get; set; }
    public bool IsActive { get; set; } = true;
}