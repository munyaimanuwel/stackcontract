# StackContract

[![NuGet](https://img.shields.io/nuget/v/stackcontract.svg)](https://www.nuget.org/packages/stackcontract)

Local-first **.NET stack/config contract checker**. Catch missing config paths, services, and env keys before runtime — **no Docker daemon required** for `validate`.

Compose is **optional**. Plain C# / ASP.NET apps can validate against `appsettings.json` (+ `appsettings.{Environment}.json`) alone.

Not affiliated with Docker, Inc. Referential mentions of Docker Compose describe the compose-file format this tool can also read.

MIT licensed. Open-core: the engine stays free forever with **zero network** in Core/Engine.

## Migration from ComposeContract

- CLI tool: `composecontract` → `stackcontract`
- Contract file: `composecontract.yml` → `stackcontract.yml` (legacy `composecontract.yml` still accepted as a deprecated alias when the new default is missing)
- Packages / namespaces: `ComposeContract.*` → `StackContract.*`

## 5-minute path (appsettings, no Docker)

```bash
# requires .NET 8 SDK
dotnet tool install -g stackcontract
cd samples/aspnet-appsettings
stackcontract validate --environment Development --strict
```

Or discover a contract from appsettings:

```bash
stackcontract init --appsettings appsettings.json --environment Development --force
stackcontract validate --environment Development --strict
```

## 5-minute path (Compose + .env)

```bash
cd samples/aspnet-compose
stackcontract init --compose compose.yml --force
stackcontract validate --strict
```

Exit codes: `0` ok/warns · `1` any error · `2` usage/parse. Use `--strict` in CI to promote warnings to errors.

## CLI

```
stackcontract init [--compose PATH] [--override PATH] [--appsettings PATH] [--environment NAME] [--options ASSEMBLY] [--force]
stackcontract validate [--contract PATH] [--profile NAME]* [--env-example PATH] [--env-local PATH] [--environment NAME] [--format text|json] [--strict]
stackcontract version
```

Finding codes: `SVC_MISSING`, `ENV_REQUIRED_MISSING`, `ENV_OPTIONAL_MISSING`, `ENV_EXAMPLE_UNKNOWN`, `CONFIG_PATH_MISSING`, `CONFIG_PATH_OPTIONAL_MISSING`, `CONFIG_PARSE`, `COMPOSE_PARSE`, `CONTRACT_INVALID`, `PROFILE_UNKNOWN`.

Local `.env` and appsettings are checked for **key/path presence only** — values never appear in text or JSON reports.

## AspNetCore

`StackContract.AspNetCore` fails host start when required configuration keys are missing (complements `ValidateOnStart`).

## GitHub Action

See [`action/action.yml`](action/action.yml) — packs/installs the tool and runs `validate --strict`.

## Sponsors / Pro

MIT core (CLI, engine, libraries, GitHub Action) stays free and local-first — **zero network** in Core/Engine.

**StackContract Pro Kit**: paid templates, CI extras, and kits. Those files are not in this repository.

Pricing and delivery will be linked here when the Polar product is live.

## What it is not

Not a Compose emulator, vault product, K8s/Helm checker, or SaaS. No phone-home in the engine.
