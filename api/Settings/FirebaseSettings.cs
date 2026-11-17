namespace SavageMetals.Api.Settings;

public class FirebaseSettings
{
    public const string SectionName = "Firebase";

    public string ProjectId { get; init; } = string.Empty;
    public string ServiceAccountPath { get; init; } = string.Empty;
    public string ServiceAccountJson { get; init; } = string.Empty;
}