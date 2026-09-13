using StackContract.AppSettings;
using StackContract.Compose;
using StackContract.Core;
using StackContract.Env;
using StackContract.Options;

namespace StackContract.Engine;

public sealed class InitService
{
    public ContractDocument Create(
        string workingDirectory,
        string? composePath = null,
        string? overridePath = null,
        string? optionsAssembly = null,
        string? appsettingsPath = null,
        string? environment = null)
    {
        var composeRel = composePath ?? "compose.yml";
        if (!File.Exists(Path.Combine(workingDirectory, composeRel)) &&
            File.Exists(Path.Combine(workingDirectory, "docker-compose.yml")))
            composeRel = "docker-compose.yml";
        var composeUsed = File.Exists(Path.Combine(workingDirectory, composeRel));

        var appsettingsRel = appsettingsPath;
        if (appsettingsRel is null && File.Exists(Path.Combine(workingDirectory, "appsettings.json")))
            appsettingsRel = "appsettings.json";
        var appsettingsUsed = appsettingsRel is not null && File.Exists(Path.Combine(workingDirectory, appsettingsRel));

        var doc = new ContractDocument
        {
            Version = 1,
            Project = new ProjectSection
            {
                Compose = composeUsed ? composeRel : null,
                Override = composeUsed ? overridePath : null
            },
            Env = new EnvSection
            {
                Example = File.Exists(Path.Combine(workingDirectory, ".env.example")) ? ".env.example" : ".env.example",
                Local = File.Exists(Path.Combine(workingDirectory, ".env")) ? ".env" : null
            },
            Config = appsettingsUsed ? new ConfigSection { File = appsettingsRel, Environment = environment } : null,
            Profiles = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = new List<string>()
            },
            Rules = new RulesSection(),
            Severity = new SeveritySection()
        };

        var composeFull = Path.Combine(workingDirectory, composeRel);
        if (composeUsed)
        {
            try
            {
                var main = ComposeParser.ParseFile(composeFull);
                ComposeFile? over = null;
                if (!string.IsNullOrWhiteSpace(overridePath))
                {
                    var op = Path.Combine(workingDirectory, overridePath);
                    if (File.Exists(op)) over = ComposeParser.ParseFile(op);
                }
                var merged = ComposeParser.Merge(main, over);
                foreach (var (name, svc) in merged.Services)
                {
                    var profiles = svc.GetProfiles();
                    if (profiles.Count == 0)
                        doc.Rules.Services.Required.Add(name);
                    else
                    {
                        foreach (var p in profiles)
                        {
                            if (!doc.Profiles.TryGetValue(p, out var list))
                            {
                                list = new List<string>();
                                doc.Profiles[p] = list;
                            }
                            if (!list.Contains(name, StringComparer.OrdinalIgnoreCase))
                                list.Add(name);
                        }
                    }
                    foreach (var key in svc.GetEnvKeys())
                    {
                        if (!doc.Rules.Env.Required.Contains(key, StringComparer.OrdinalIgnoreCase))
                            doc.Rules.Env.Required.Add(key);
                    }
                }
            }
            catch
            {
                // leave empty rules; validate will surface compose parse errors
            }
        }

        if (appsettingsUsed)
        {
            var baseFull = Path.IsPathRooted(appsettingsRel!) ? appsettingsRel! : Path.Combine(workingDirectory, appsettingsRel!);
            var discovered = AppSettingsParser.ParseFile(baseFull);
            if (!string.IsNullOrWhiteSpace(environment))
            {
                var envRel = Path.Combine(Path.GetDirectoryName(appsettingsRel!) ?? string.Empty, Path.GetFileNameWithoutExtension(appsettingsRel!) + "." + environment + Path.GetExtension(appsettingsRel!));
                var envFull = Path.Combine(workingDirectory, envRel);
                if (File.Exists(envFull)) discovered = AppSettingsParser.Merge(discovered, AppSettingsParser.ParseFile(envFull));
            }
            foreach (var path in discovered.Paths)
                if (!doc.Rules.Config.Optional.Contains(path, StringComparer.OrdinalIgnoreCase) &&
                    !doc.Rules.Config.Required.Contains(path, StringComparer.OrdinalIgnoreCase))
                    doc.Rules.Config.Optional.Add(path);
        }

        var examplePath = Path.Combine(workingDirectory, doc.Env.Example);
        if (File.Exists(examplePath))
        {
            foreach (var key in EnvParser.ParseFile(examplePath).Keys)
            {
                if (!doc.Rules.Env.Required.Contains(key, StringComparer.OrdinalIgnoreCase) &&
                    !doc.Rules.Env.Optional.Contains(key, StringComparer.OrdinalIgnoreCase))
                    doc.Rules.Env.Optional.Add(key);
            }
        }

        if (!string.IsNullOrWhiteSpace(optionsAssembly))
        {
            var asmPath = Path.IsPathRooted(optionsAssembly)
                ? optionsAssembly
                : Path.Combine(workingDirectory, optionsAssembly);
            if (File.Exists(asmPath))
            {
                doc.Rules.Options.Assemblies.Add(optionsAssembly);
                foreach (var key in OptionsKeyExtractor.ExtractFromAssembly(asmPath))
                {
                    if (!doc.Rules.Env.Required.Contains(key, StringComparer.OrdinalIgnoreCase))
                        doc.Rules.Env.Required.Add(key);
                }
            }
        }

        return doc;
    }
}
