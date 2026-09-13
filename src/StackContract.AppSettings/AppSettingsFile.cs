namespace StackContract.AppSettings;

public sealed class AppSettingsFile
{
    /// <summary>Flattened configuration paths only — values are never retained.</summary>
    public HashSet<string> Paths { get; } = new(StringComparer.OrdinalIgnoreCase);
}
