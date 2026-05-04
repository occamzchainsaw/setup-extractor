# Copilot Instructions

## Build and test

Use the solution file at the repo root:

```powershell
dotnet build .\IRacingSetupExtractor.slnx
dotnet test .\IRacingSetupExtractor.slnx
```

Run a single NUnit test with `--filter`, for example:

```powershell
dotnet test .\Extractor.Tests\Extractor.Tests.csproj --filter "FullyQualifiedName~Extractor.Tests.CarMatcherTests.ShouldMatchExactFolderName"
```

No dedicated lint or format command is defined in the repository. Diagnostics currently come from the .NET SDK build and the analyzers referenced by the projects.

## High-level architecture

- `Extractor.Gui` is the main application. `App.axaml.cs` builds the DI container manually, registers the repositories and matcher services from `Extractor.Core`, and loads `coreConfig.json`, `tracksData.json`, and `setupShopsData.json` from the app base directory.
- `Extractor.Core` contains the matching and path-building logic. `CarMatcher` derives valid car names from directories under `CoreConfig.SetupsBasePath`, while `TrackMatcher` and `SetupShopMatcher` fuzzy-match archive path segments against JSON-backed alias data. `PathContextComposer` runs those matchers and produces a `PathTemplateContext`, and `PathGenerator` expands that into the destination path.
- `Extractor.Gui\ViewModels` edits the JSON-backed data through repository interfaces, with AutoMapper translating between core models and GUI DTOs.
- Navigation in the GUI is simple page swapping: `MainWindowViewModel.CurrentPage` selects the active view model, and `ViewLocator` resolves `*ViewModel` to the matching `*View` by naming convention.
- `Extractor.Tests` currently exercises matcher behavior directly without the app container. Tests construct services with `MockOptionsMonitor<T>` instead of using the GUI registration path.
- `Extractor.ApiClient` is a standalone OAuth/iRacing data-fetch console utility, and `Extractor.HelperConsole` is a one-off data generation tool. Neither is part of the normal GUI runtime flow.

## Key conventions

- Treat `coreConfig.json`, `tracksData.json`, and `setupShopsData.json` as runtime-owned inputs. If you rename, move, or add config/data files, update both `App.axaml.cs` and `Extractor.Gui.csproj` so the files are still loaded and copied to the output directory.
- Path template placeholders are literal `PathElement` enum names such as `Track`, `Season`, `SeasonAndWeek`, and `SetupShop`. `PathGenerator` uses direct string replacement rather than a templating engine.
- Matching is segment-based and normalized with `SanitizeSpecialChars()`, which lowercases text and strips non-alphanumeric characters before fuzzy matching. Keep that normalization path in mind when changing match logic or fixtures.
- GUI screens that load data do it from the view code-behind `OnLoaded` handlers (`SettingsView.axaml.cs`, `TracksView.axaml.cs`), which call `InitializeAsync()` once using an `_isInitialized` guard in the view model. Loading behavior is split across the view and the view model.
- The codebase targets .NET 10, and `Extractor.Core\Extensions\StringExtensions.cs` uses the newer extension member syntax. Keep SDK and language-version assumptions aligned with `net10.0` when changing shared helpers.
- The current test suite is not fully repo-relative: `TrackMatcherTests` reads `tracksData.json` from a hardcoded `/home/paul/dev/setup-extractor/tracksData.json` path, so `dotnet test .\IRacingSetupExtractor.slnx` currently fails on Windows until that fixture path is made repository-relative.
