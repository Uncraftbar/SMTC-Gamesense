using System.ComponentModel;
using Windows.Media.Control;

namespace SMTCGamesense;

public class SMTCGamesenseApp : ApplicationContext
{
    private NotifyIcon? _trayIcon;
    private System.Windows.Forms.Timer? _timer;
    private MediaSessionMonitor? _mediaMonitor;
    private GamesenseClient? _gamesenseClient;
    private string _currentTooltip = "SMTC Gamesense - No media playing";

    public SMTCGamesenseApp()
    {
        InitializeTrayIcon();
        InitializeComponents();
        StartMonitoring();
    }

    private void InitializeTrayIcon()
    {
        _trayIcon = new NotifyIcon()
        {
            Icon = SystemIcons.Application, // We'll replace this with a custom icon later
            ContextMenuStrip = CreateContextMenu(),
            Visible = true,
            Text = _currentTooltip
        };
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var contextMenu = new ContextMenuStrip();
        var exitMenuItem = new ToolStripMenuItem("Exit", null, OnExit);
        contextMenu.Items.Add(exitMenuItem);
        return contextMenu;
    }

    private void InitializeComponents()
    {
        _mediaMonitor = new MediaSessionMonitor();
        _gamesenseClient = new GamesenseClient();
        
        // Timer to check media status every second
        _timer = new System.Windows.Forms.Timer()
        {
            Interval = 1000, // 1 second
            Enabled = true
        };
        _timer.Tick += OnTimerTick;
    }

    private void StartMonitoring()
    {
        _timer?.Start();
    }

    private async void OnTimerTick(object? sender, EventArgs e)
    {
        try
        {
            var mediaInfo = await _mediaMonitor!.GetCurrentMediaInfoAsync();
            
            if (mediaInfo != null && !string.IsNullOrEmpty(mediaInfo.Title))
            {
                // Update tooltip
                var newTooltip = $"SMTC Gamesense - {mediaInfo.Artist} - {mediaInfo.Title}";
                if (_currentTooltip != newTooltip)
                {
                    _currentTooltip = newTooltip;
                    if (_trayIcon != null)
                        _trayIcon.Text = _currentTooltip.Length > 63 ? _currentTooltip[..60] + "..." : _currentTooltip;
                }

                // Send to Gamesense
                await _gamesenseClient!.SendMediaInfoAsync(mediaInfo);
            }
            else
            {
                // No media playing
                var newTooltip = "SMTC Gamesense - No media playing";
                if (_currentTooltip != newTooltip)
                {
                    _currentTooltip = newTooltip;
                    if (_trayIcon != null)
                        _trayIcon.Text = _currentTooltip;
                }
            }
        }
        catch (Exception ex)
        {
            // Handle errors silently or log them
            System.Diagnostics.Debug.WriteLine($"Error in timer tick: {ex.Message}");
        }
    }

    private void OnExit(object? sender, EventArgs e)
    {
        _timer?.Stop();
        _timer?.Dispose();
        _trayIcon?.Dispose();
        ExitThread();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer?.Dispose();
            _trayIcon?.Dispose();
            _mediaMonitor?.Dispose();
            _gamesenseClient?.Dispose();
        }
        base.Dispose(disposing);
    }
}
