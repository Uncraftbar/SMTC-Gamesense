# SMTC-Gamesense

A C#/.NET 8 Windows desktop application that runs in the system tray and integrates Windows System Media Transport Controls (SMTC) with SteelSeries Gamesense.

## Features

- **System Tray Application**: Runs minimized in the system tray with a context menu
- **SMTC Integration**: Monitors currently playing media using Windows System Media Transport Controls
- **Gamesense Display**: Sends artist and title information to SteelSeries Gamesense for keyboard OLED display
- **Real-time Updates**: Checks for media changes every second
- **Smart Display**: Automatically truncates long text to fit OLED display limitations

## Requirements

- Windows 10/11 x64
- .NET 8 Runtime
- SteelSeries Engine with Gamesense-compatible device (keyboard with OLED display)

## How It Works

1. The application starts and creates a system tray icon
2. Every second, it checks the current media session using SMTC
3. If media is playing, it extracts the artist and title
4. The information is sent to SteelSeries Gamesense via REST API (http://127.0.0.1:3000)
5. Gamesense displays the media info on the keyboard OLED
6. The tray icon tooltip shows the current track or status

## Building and Running

### Prerequisites
- .NET 8 SDK
- Windows 10 version 1809 (17763) or later

### Build
```powershell
dotnet build
```

### Run
```powershell
dotnet run
```

Or run the executable directly:
```powershell
.\bin\Debug\net8.0-windows10.0.17763.0\SMTCGamesense.exe
```

## Architecture

### Main Components

- **Program.cs**: Application entry point
- **SMTCGamesenseApp.cs**: Main application logic, system tray management, and coordination
- **MediaSessionMonitor.cs**: SMTC integration for reading Windows media information
- **GamesenseClient.cs**: SteelSeries Gamesense REST API client

### Key Classes

#### `SMTCGamesenseApp`
- Inherits from `ApplicationContext` for proper WinForms tray application behavior
- Manages system tray icon and context menu
- Coordinates between media monitoring and Gamesense communication
- Handles the main timer loop for periodic updates

#### `MediaSessionMonitor`
- Uses `GlobalSystemMediaTransportControlsSessionManager` for SMTC access
- Extracts artist, title, and playback status from current media session
- Handles cases where no media is playing or SMTC is unavailable

#### `GamesenseClient`
- Implements SteelSeries Gamesense REST API protocol
- Handles game registration, event binding, and data sending
- Manages HTTP communication with the local Gamesense service
- Formats media information for OLED display

## SteelSeries Gamesense Integration

The application registers itself as a game called "SMTC Media Player" and creates a custom event for media information display. The data is formatted to show:

- Line 1: Artist name (truncated to fit OLED width)
- Line 2: Song title (truncated to fit OLED width)

## Usage

1. Ensure SteelSeries Engine is running
2. Start the SMTC-Gamesense application
3. Play media in any Windows application (Spotify, Windows Media Player, browsers, etc.)
4. Check your SteelSeries keyboard OLED for media information display
5. Right-click the system tray icon to exit the application

## Dependencies

- **Newtonsoft.Json**: For REST API communication with Gamesense
- **Windows Runtime (WinRT)**: For SMTC access via `Windows.Media.Control` namespace

## Troubleshooting

- **No media information**: Ensure the media player supports SMTC (most modern players do)
- **Gamesense not working**: Verify SteelSeries Engine is running and Gamesense is enabled
- **Build errors**: Ensure you're using .NET 8 SDK and Windows 10/11

## License

This project is provided as-is for educational and personal use.
