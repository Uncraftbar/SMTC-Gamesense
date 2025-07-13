using Newtonsoft.Json;
using System.Text;

namespace SMTCGamesense;

public class GamesenseClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _gameId = "SMTC_GAMESENSE";
    private readonly string _eventId = "MEDIA_PLAYING";
    private readonly string _gamesenseEndpoint = "http://127.0.0.1:51130"; // Default Gamesense endpoint
    private bool _isRegistered = false;

    public GamesenseClient()
    {
        _httpClient = new HttpClient();
    }

    public async Task SendMediaInfoAsync(MediaInfo mediaInfo)
    {
        try
        {
            if (!_isRegistered)
            {
                await RegisterGameAsync();
                await RegisterEventAsync();
                _isRegistered = true;
            }

            await SendEventAsync(mediaInfo);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error sending to Gamesense: {ex.Message}");
        }
    }

    private async Task RegisterGameAsync()
    {
        var gameData = new
        {
            game = _gameId,
            game_display_name = "SMTC Media Player",
            developer = "SMTC-Gamesense"
        };

        var json = JsonConvert.SerializeObject(gameData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await _httpClient.PostAsync($"{_gamesenseEndpoint}/game_metadata", content);
    }

    private async Task RegisterEventAsync()
    {
        var eventData = new
        {
            game = _gameId,
            @event = _eventId,
            min_value = 0,
            max_value = 1,
            icon_id = 15, // Music note icon
            handlers = new[]
            {
                new
                {
                    device_type = "keyboard",
                    zone = "function-keys",
                    mode = "screen",
                    datas = new[]
                    {
                        new
                        {
                            has_text = true,
                            context_frame_key = "media-info"
                        }
                    }
                }
            }
        };

        var json = JsonConvert.SerializeObject(eventData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await _httpClient.PostAsync($"{_gamesenseEndpoint}/bind_game_event", content);
    }

    private async Task SendEventAsync(MediaInfo mediaInfo)
    {
        var eventData = new
        {
            game = _gameId,
            @event = _eventId,
            data = new
            {
                value = 1,
                frame = new Dictionary<string, object>
                {
                    ["media-info"] = new
                    {
                        lines = new[]
                        {
                            new
                            {
                                type = "text",
                                data = mediaInfo.Artist.Length > 20 ? mediaInfo.Artist[..17] + "..." : mediaInfo.Artist,
                                size = 9
                            },
                            new
                            {
                                type = "text", 
                                data = mediaInfo.Title.Length > 20 ? mediaInfo.Title[..17] + "..." : mediaInfo.Title,
                                size = 9
                            }
                        }
                    }
                }
            }
        };

        var json = JsonConvert.SerializeObject(eventData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await _httpClient.PostAsync($"{_gamesenseEndpoint}/game_event", content);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
