<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# SMTC-Gamesense Project Instructions

This is a C#/.NET 8 Windows desktop application that:

- Runs in the system tray and monitors Windows System Media Transport Controls (SMTC)
- Periodically reads media playback information using GlobalSystemMediaTransportControlsSessionManager
- Sends current song artist and title to SteelSeries Gamesense via REST API
- Uses WinForms for tray functionality with exit menu option

## Key Components:
- **SMTCGamesenseApp.cs**: Main application class handling system tray and coordination
- **MediaSessionMonitor.cs**: SMTC integration for reading media information
- **GamesenseClient.cs**: SteelSeries Gamesense REST API client
- **Program.cs**: Application entry point

## Technical Requirements:
- .NET 8 with Windows 10/11 x64 target (net8.0-windows10.0.17763.0)
- Windows Runtime (WinRT) APIs for SMTC access
- Minimal dependencies (Newtonsoft.Json for API communication)
- System tray functionality with context menu

## Development Guidelines:
- Keep error handling robust for media session access
- Ensure proper disposal of resources
- Handle cases where no media is playing
- Truncate long artist/title names for display
- Use async/await patterns for API calls
