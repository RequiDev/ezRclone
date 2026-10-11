[![.NET Core Desktop](https://github.com/RequiDev/ezRclone/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/RequiDev/ezRclone/actions/workflows/dotnet-desktop.yml)

# ezRclone

Easily mount [rclone](https://rclone.org) remotes as drives on Windows.

ezRclone lives in the notification area and turns the remotes you configured with `rclone config` into drive
letters, so you don't need to keep a terminal open per mount.

## Features

- **Your rclone remotes, picked up automatically**: the remote list comes straight from rclone, including the
  remote type. Remotes you delete from rclone are flagged and can be removed.
- **One-click mounting** from the window or the tray menu, plus *Mount all* / *Unmount all*.
- **Auto-mount on startup** per remote, and an option to start ezRclone with Windows, minimized to the tray.
- **Per-remote options**: drive letter (only free letters are offered), sub folder of the remote, volume name,
  network drive mode, VFS cache mode and any extra `rclone mount` flags, with a preview of the full command.
- **Clear status and errors**: each remote shows whether it is mounting, mounted or failed and why. The *Log* tab
  shows rclone's output, and a tray notification tells you when a mount fails or stops unexpectedly.
- **Survives restarts**: mounts keep running when ezRclone exits and are picked up again on the next start.
  If you prefer, ezRclone can unmount everything on exit instead.
- **Open in Explorer** for mounted drives, light and dark mode following Windows, and a single instance: starting
  ezRclone again brings the running window to the front.

## Requirements

- Windows 10 or 11
- [.NET 9 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/9.0)
- [rclone](https://rclone.org/downloads/) with at least one configured remote
- [WinFsp](https://winfsp.dev/rel/), which rclone needs to mount drives on Windows

## Getting started

1. Download `ezRclone.exe` from the [latest release](https://github.com/RequiDev/ezRclone/releases/latest).
2. Start it. If rclone is not on your `PATH`, open *Settings* and point ezRclone at the folder containing
   `rclone.exe`.
3. Select a remote, pick a drive letter and press *Mount*. Tick *Mount when ezRclone starts* to mount it
   automatically from now on.

Closing the window keeps ezRclone running in the tray. Use *Exit* in the tray menu to quit.

### Command line

| Argument   | Effect                                                              |
|------------|---------------------------------------------------------------------|
| `--hidden` | Start minimized to the tray. Used by *Start minimized to the tray*. |

### Where settings are stored

Settings are saved to `%APPDATA%\ezRclone\settings.json`. Settings of older ezRclone versions, which were stored in
a version specific sub folder, are migrated automatically on first start. Autostart uses the `ezRclone` value in
`HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run`.

## Building

You need the [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).

```sh
dotnet build ezRclone.sln
dotnet test ezRclone.sln
dotnet publish ezRclone/ezRclone.csproj -r win-x64 -c Release -p:PublishSingleFile=true --self-contained false
```

The app only runs on Windows, but everything builds on Linux and macOS too, and the core tests run there. The UI
smoke test in `ezRclone.UITests` only runs on Windows.

| Project            | Contents                                                                         |
|--------------------|----------------------------------------------------------------------------------|
| `ezRclone`         | WPF app: window, tray icon, view models, Windows integration (registry, WMI)     |
| `ezRclone.Core`    | Platform independent logic: settings, rclone commands, mount process management  |
| `ezRclone.Tests`   | xUnit tests for `ezRclone.Core`                                                  |
| `ezRclone.UITests` | Windows only smoke test that loads the main window and fails on binding errors   |

## License

[GPL-3.0](LICENSE)
