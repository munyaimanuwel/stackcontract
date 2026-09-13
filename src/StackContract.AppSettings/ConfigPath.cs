namespace StackContract.AppSettings;

public static class ConfigPath
{
    /// <summary>Canonical form for matching: ':' is primary, '__' is accepted as an alias.</summary>
    public static string Normalize(string path) => path.Replace("__", ":").Trim();
}
