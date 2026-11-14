namespace SavageMetals.Api.Settings;

public class CorsSettings
{
    public const string SectionName = "Cors";
    public const string PolicyName = "Frontend";

    public string[] AllowedOrigins { get; init; } = [];
}