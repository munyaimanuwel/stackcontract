namespace StackContract.Engine;
public sealed class ValidationOptions
{
    public string ContractPath { get; init; } = "stackcontract.yml";
    public IReadOnlyList<string> Profiles { get; init; } = Array.Empty<string>();
    public string? EnvExamplePath { get; init; }
    public string? EnvLocalPath { get; init; }
    public string? Environment { get; init; }
    public bool Strict { get; init; }
    public string WorkingDirectory { get; init; } = Directory.GetCurrentDirectory();
}
