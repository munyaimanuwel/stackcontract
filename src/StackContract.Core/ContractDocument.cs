namespace StackContract.Core;

public sealed class ContractDocument
{
    public int Version { get; set; } = 1;
    public ProjectSection Project { get; set; } = new();
    public EnvSection Env { get; set; } = new();
    public ConfigSection? Config { get; set; }
    public Dictionary<string, List<string>> Profiles { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public RulesSection Rules { get; set; } = new();
    public SeveritySection Severity { get; set; } = new();
}

public sealed class ProjectSection
{
    public string? Compose { get; set; }
    public string? Override { get; set; }
}

public sealed class EnvSection
{
    public string Example { get; set; } = ".env.example";
    public string? Local { get; set; }
}

public sealed class ConfigSection
{
    public string? File { get; set; } = "appsettings.json";
    public string? Environment { get; set; }
}

public sealed class RulesSection
{
    public ServiceRules Services { get; set; } = new();
    public EnvRules Env { get; set; } = new();
    public ConfigRules Config { get; set; } = new();
    public OptionsRules Options { get; set; } = new();
}

public sealed class ServiceRules
{
    public List<string> Required { get; set; } = new();
}

public sealed class EnvRules
{
    public List<string> Required { get; set; } = new();
    public List<string> Optional { get; set; } = new();
}

public sealed class ConfigRules
{
    public List<string> Required { get; set; } = new();
    public List<string> Optional { get; set; } = new();
}

public sealed class OptionsRules
{
    public List<string> Assemblies { get; set; } = new();
    public List<string> Types { get; set; } = new();
}

public sealed class SeveritySection
{
    public string MissingService { get; set; } = "error";
    public string MissingEnvRequired { get; set; } = "error";
    public string MissingEnvOptional { get; set; } = "warn";
    public string UnknownEnvInExample { get; set; } = "warn";
    public string MissingConfigRequired { get; set; } = "error";
    public string MissingConfigOptional { get; set; } = "warn";
}
