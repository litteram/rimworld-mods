# AGENTS.md - OrganizedResearchTech

## Overview

RimWorld 1.6 mod that organizes research tabs in a sensible order. Uses Harmony for runtime patching, targets .NET Framework 4.7.2.

## Build Commands

```bash
# From Source directory
msbuild OrganizedResearchTech.csproj

# Single file build
msbuild OrganizedResearchTech.csproj /p:Configuration=Debug /t:Build

# Clean build
msbuild OrganizedResearchTech.csproj /t:Clean,Build
```

Output: `../1.6/Assemblies/OrganizedResearchTech.dll` (both Debug and Release)

## Testing

**No unit tests exist.** Manual testing via RimWorld game client.

## Code Style

### Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Classes/Methods/Properties | PascalCase | `OrganizedResearchTech`, `DoSettingsWindowContents` |
| Private fields | camelCase | `weigts` (typo in original) |
| Harmony patches | `<Class>_<Method>` | `MainTabWindow_Research_PatchPostOpen` |

### Import Order

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;           // RimWorld core
using HarmonyLib;       // Harmony patches
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
```

System -> Third-party -> Game (RimWorld, Unity)

### Harmony Patterns

```csharp
[HarmonyPatch(typeof(TargetClass), "TargetMethod")]
[UsedImplicitly]
class ClassName_PatchMethod
{
    [UsedImplicitly]
    static void Prefix(...) { }
    [UsedImplicitly]
    static void Postfix(...) { }
    [UsedImplicitly]
    static IEnumerable<CodeInstruction> Transpiler(...) { }
}
```

Always include `[UsedImplicitly]` on patch methods. Use `ref` for private fields (e.g., `ref List<...> ___tabs`).

### RimWorld API

- `DefDatabase<T>.AllDefsListForReading` for iterating defs
- `Scribe_Values.Look(...)` for settings persistence
- Mod classes extend `Mod`, settings extend `ModSettings`
- Use `Log.Message()`, `Log.Warning()`, `Log.Error()` - not Console.WriteLine

### Type Usage

- Prefer interfaces: `IComparer<T>`, `IEnumerable<T>`
- Use RimWorld types: `List<T>`, `Dictionary<K,V>`
- Use `ref` for modifying ref parameters

### ModSettings Pattern

```csharp
public class ModSettings : ModSettings
{
    public bool SomeSetting;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref SomeSetting, "settingName");
    }
}
```

### Harmony Initialization

```csharp
[StaticConstructorOnStartup]
public static class ModStartup
{
    static ModStartup()
    {
        new Harmony("zenlor.OrganizedResearchTech").PatchAll(Assembly.GetExecutingAssembly());
    }
}
```

## Project Structure

```
OrganizedResearchTech/
├── Source/
│   ├── OrganizedResearchTech.cs
│   ├── ResearchComparer.cs
│   ├── Properties/AssemblyInfo.cs
│   └── OrganizedResearchTech.csproj
├── 1.6/Assemblies/      # DLL output
├── Defs/                 # XML definitions
├── About/                # Mod metadata
└── Patches/              # Harmony XML files
```

## Dependencies

- **Krafs.Rimworld.Ref** (v1.6.4633) - RimWorld API
- **Krafs.Publicizer** (v2.3.0) - Access private members
- **Lib.Harmony** (v2.3.6) - Runtime patching
- **JetBrains.Annotations** - ReSharper attributes

## Notes

1. Harmony ID: `zenlor.OrganizedResearchTech`
2. DLL goes to `1.6/Assemblies/` for RimWorld loading
3. Patches `MainTabWindow_Research.PostOpen`
4. Uses publicized RimWorld assemblies

## RimWorld Initialization Order (Important!)

**DefDatabase is NOT available during mod construction or ExposeData.**

```
1. Mod constructor runs → DefDatabase empty
2. ExposeData runs (load settings) → DefDatabase still empty
3. Game finishes loading
4. LongEventHandler.ExecuteWhenFinished callbacks fire → DefDatabase ready
```

### Wrong Pattern

```csharp
public OrganizedResearchTech(ModContentPack content) : base(content)
{
    Settings = GetSettings<OrganizedResearchTechSettings>();
    // WRONG: DefDatabase not ready yet!
    allTabs = DefDatabase<ResearchTabDef>.AllDefsListForReading;
    ResearchTabs.Initialize(Settings);
}
```

### Correct Pattern

```csharp
public OrganizedResearchTech(ModContentPack content) : base(content)
{
    Settings = GetSettings<OrganizedResearchTechSettings>();
    LongEventHandler.ExecuteWhenFinished(InitializeCache);
}

private void InitializeCache()
{
    // DefDatabase is now ready
    allTabs = DefDatabase<ResearchTabDef>.AllDefsListForReading;
    ResearchTabs.Initialize(Settings);
}
```

## ModSettings Best Practices

### Store defNames, not Def objects

Def objects are not serializable and may not exist during loading. Store defName strings and resolve them at runtime.

```csharp
// Good: Store strings
public List<string> TabDefNames = new List<string>();

public override void ExposeData()
{
    base.ExposeData();
    Scribe_Collections.Look(ref TabDefNames, "ResearchTabList", LookMode.Value);
}

// Resolve at runtime when needed
public ResearchTabDef GetTabDef(string defName)
{
    return DefDatabase<ResearchTabDef>.GetNamed(defName, errorOnFail: false);
}
```

### ExposeData Modes

```csharp
public override void ExposeData()
{
    base.ExposeData();
    Scribe_Collections.Look(ref data, "key", LookMode.Value);
    
    // PostLoadInit runs after data is loaded
    if (Scribe.mode == LoadSaveMode.PostLoadInit && data == null)
    {
        data = GetDefaults();
    }
}
```

## Linting

No formal linter. JetBrains Rider/ReSharper highlights issues. Run build to verify.
