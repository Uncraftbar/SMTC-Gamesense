using Windows.Media.Control;

namespace SMTCGamesense;

public class MediaInfo
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public bool IsPlaying { get; set; }
}

public class MediaSessionMonitor : IDisposable
{
    private GlobalSystemMediaTransportControlsSessionManager? _sessionManager;

    public async Task<MediaInfo?> GetCurrentMediaInfoAsync()
    {
        try
        {
            _sessionManager ??= await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

            var currentSession = _sessionManager.GetCurrentSession();
            if (currentSession == null)
                return null;

            var playbackInfo = currentSession.GetPlaybackInfo();
            if (playbackInfo.PlaybackStatus != GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                return null;

            var mediaProperties = await currentSession.TryGetMediaPropertiesAsync();
            if (mediaProperties == null)
                return null;

            return new MediaInfo
            {
                Title = mediaProperties.Title ?? string.Empty,
                Artist = mediaProperties.Artist ?? string.Empty,
                IsPlaying = playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting media info: {ex.Message}");
            return null;
        }
    }

    public void Dispose()
    {
        // GlobalSystemMediaTransportControlsSessionManager doesn't implement IDisposable
        // Nothing to dispose here
    }
}
