# AGENTS.md

Guidance for coding agents (and humans) working on ezRclone, a Windows tray app that mounts rclone remotes as
drive letters. See [README.md](README.md) for what the app does from a user's point of view.

## Layout

- `ezRclone/`: WPF app (`net9.0-windows`).
  - `App.xaml.cs`: startup, single instance, window and tray lifetime, exit.
  - `MainWindow.xaml`: the only window, with the Remotes, Log and Settings tabs.
  - `ViewModels/`: `MainViewModel` (remote list, settings, log), `RemoteViewModel` (one remote), plus the small
    hand-written MVVM helpers `ObservableObject` and `RelayCommand`/`AsyncRelayCommand`.
  - `Services/`: Windows integration: `RegistryAutostart`, `ExistingMounts` (WMI command line lookup),
    `SingleInstance`.
  - `TrayIcon.cs`: wraps the WinForms `NotifyIcon`, because WPF has none of its own.
- `ezRclone.Core/`: platform-independent logic (`net9.0`), no UI types.
  - `SettingsStore`: JSON settings in `%APPDATA%\ezRclone`, including migration from the old WinForms location.
  - `Rclone`: short-lived commands (`listremotes --long`, `version`).
  - `MountArguments`: builds `rclone mount` arguments, splits extra flags, matches existing command lines.
  - `MountManager`: starts, stops, adopts and watches `rclone mount` processes.
  - `DriveLetters`: normalizing and finding free drive letters.
- `ezRclone.Tests/`: xUnit tests for Core. `FakeRclone` writes a `.cmd` or `.sh` script that stands in for rclone.
- `ezRclone.UITests/`: Windows-only smoke test that loads the real `App` resources and `MainWindow`, visits every
  tab and fails on any WPF binding error.

## Commands

```sh
dotnet build ezRclone.sln
dotnet test ezRclone.sln
```

- Building works on Linux and macOS (`EnableWindowsTargeting` is set). There, `dotnet test` runs only
  `ezRclone.Tests`; `ezRclone.UITests` is turned off outside Windows (`IsTestProject` is false).
- The app itself cannot run outside Windows. CI (`.github/workflows/dotnet-desktop.yml`) runs all tests on
  `windows-latest` for every pushed branch. It is the place where the UI smoke test actually runs, so check it
  after changing XAML.
- Release builds are published as a single-file, framework-dependent `ezRclone.exe`. Pushes to `master` tag and
  publish a GitHub release automatically.

## Architecture notes

- Put logic in `ezRclone.Core` whenever it doesn't need Windows or WPF, and cover it with tests there. The app
  project should be wiring and presentation.
- `MountManager` raises `StateChanged` and `OutputReceived` on background threads. `MainViewModel` marshals them to
  the dispatcher; keep it that way and never touch view models from those handlers directly.
- An rclone exit is only "expected" if `UnmountAsync` asked for it. Any other exit becomes `MountStatus.Failed`,
  with a message taken from rclone's last `CRITICAL`/`ERROR` line. That drives the status text, the log and the
  tray notification.
- Mounts are considered ready when the drive root exists, or once `ReadyTimeout` passes with rclone still running.
- Mounts intentionally outlive ezRclone (unless *Unmount on exit* is on). On startup, `ExistingMounts` finds
  running `rclone mount` processes via WMI and hands them to `MountManager.Adopt`.
- Pass process arguments through `ProcessStartInfo.ArgumentList`, never by building a quoted string.
- Remotes are listed with `rclone listremotes --long`. Don't parse `rclone.conf` yourself: it's INI rather than
  TOML, and it can be encrypted.

## Compatibility rules

- `settings.json` must keep loading files written by older versions. Don't rename or remove properties of
  `Settings` or `Mountable`; add new ones with sensible defaults (missing properties keep their initializer value).
- The autostart registry value is named `ezRclone`, and `--hidden` means "start in the tray". Both are shared with
  older versions.
- The published executable must stay `ezRclone.exe`, because existing autostart entries point at it.

## WPF gotchas

- The project enables both WPF and WinForms. The WinForms and System.Drawing global usings are removed in the
  csproj because they clash with WPF types. Reference WinForms explicitly (`using Forms = System.Windows.Forms;`),
  and only in `TrayIcon.cs`.
- Reference resources with assembly-qualified pack URIs (`pack://application:,,,/ezRclone;component/...`). Short
  forms such as `/rclone.ico` resolve against the entry assembly and break in tests.
- The Fluent theme is enabled with `ThemeMode="System"` in `App.xaml` (experimental in .NET 9, hence
  `NoWarn WPF0001`). Don't depend on theme resource keys with `StaticResource`; a missing key crashes at runtime.
  Use the shared resources in `App.xaml` or local values instead.
- Bindings to read-only properties need `Mode=OneWay`, notably on `TextBox.Text` and `Run.Text`.

## Conventions

- C#: file-scoped namespaces, nullable enabled, `_camelCase` private fields, braces omitted only for single-line
  `if` bodies (as in the existing code). Match the surrounding code and keep comments for the non-obvious "why".
- Avoid new NuGet dependencies unless they clearly pay for themselves; the app currently needs only
  `System.Management`.
- Branches: `fix/<topic>`, `feature/<topic>`, `docs/<topic>`, `ci/<topic>`.
- Keep commits atomic: one responsibility per commit, with the tests that cover it.
- Commit messages are a single short sentence, `Area: Past tense description.`, for example
  `UI: Fixed double dispose during mount.` Areas in use: `Core`, `Mounts`, `UI`, `Tray`, `Settings`, `App`,
  `Tests`, `CI`, `Docs`.
