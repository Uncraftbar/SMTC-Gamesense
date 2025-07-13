# Quick Setup Guide

## Requirements
- Windows 10/11 x64
- .NET 8 Runtime or SDK
- SteelSeries Engine with Gamesense-compatible keyboard

## Installation
1. Download or clone this repository
2. Open PowerShell/Command Prompt in the project folder
3. Run: `dotnet build`
4. Run: `dotnet run` or execute `.\bin\Debug\net8.0-windows10.0.17763.0\SMTCGamesense.exe`

## First Run
1. Start SteelSeries Engine
2. Run the SMTC-Gamesense application
3. Look for the system tray icon (should appear near clock)
4. Play any media (Spotify, YouTube, Windows Media Player, etc.)
5. Check your SteelSeries keyboard OLED for media information

## Exiting
- Right-click the system tray icon and select "Exit"

## Troubleshooting
- If no tray icon appears, check if the app is running in Task Manager
- If no media info shows on keyboard, ensure SteelSeries Engine is running
- If build fails, ensure you have .NET 8 SDK installed
