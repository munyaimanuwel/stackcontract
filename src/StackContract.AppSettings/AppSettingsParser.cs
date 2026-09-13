using System.Text.Json;

namespace StackContract.AppSettings;

public static class AppSettingsParser
{
    public static AppSettingsFile ParseFile(string path)
    {
        if (!File.Exists(path)) return new AppSettingsFile();
        return Parse(File.ReadAllText(path));
    }

    public static AppSettingsFile Parse(string json)
    {
        var file = new AppSettingsFile();
        using var doc = JsonDocument.Parse(json);
        Walk(doc.RootElement, string.Empty, file.Paths);
        return file;
    }

    /// <summary>Presence-only union; later files add paths, they never remove them.</summary>
    public static AppSettingsFile Merge(params AppSettingsFile[] files)
    {
        var merged = new AppSettingsFile();
        foreach (var file in files)
            foreach (var path in file.Paths)
                merged.Paths.Add(path);
        return merged;
    }

    private static void Walk(JsonElement element, string prefix, HashSet<string> paths)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            if (prefix.Length > 0) paths.Add(prefix);
            return;
        }

        foreach (var property in element.EnumerateObject())
        {
            var path = prefix.Length == 0 ? property.Name : prefix + ":" + property.Name;
            if (property.Value.ValueKind == JsonValueKind.Object)
                Walk(property.Value, path, paths);
            else
                paths.Add(path);
        }
    }
}
