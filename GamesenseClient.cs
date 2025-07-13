using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace SMTCGamesense;

public class GamesenseClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _gameId = "SMTC_GAMESENSE";
    private readonly string _eventId = "MEDIA_PLAYING";
    private string _gamesenseEndpoint = "http://127.0.0.1:3000"; // Default fallback
    private bool _isRegistered = false;

    public GamesenseClient()
    {
        _httpClient = new HttpClient();
        LoadGamesenseEndpoint();
    }

    private void LoadGamesenseEndpoint()
    {
        try
        {
            var corePropsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "SteelSeries", "SteelSeries Engine 3", "coreProps.json");

            if (File.Exists(corePropsPath))
            {
                var jsonContent = File.ReadAllText(corePropsPath);
                var coreProps = JObject.Parse(jsonContent);
                var address = coreProps["address"]?.ToString();

                if (!string.IsNullOrEmpty(address))
                {
                    _gamesenseEndpoint = $"http://{address}";
                    System.Diagnostics.Debug.WriteLine($"Gamesense endpoint loaded: {_gamesenseEndpoint}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No address found in coreProps.json, using default endpoint");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"coreProps.json not found at: {corePropsPath}, using default endpoint");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading Gamesense endpoint: {ex.Message}, using default");
        }
    }

    public async Task SendMediaInfoAsync(MediaInfo mediaInfo)
    {
        try
        {
            if (!_isRegistered)
            {
                System.Diagnostics.Debug.WriteLine($"Registering game with Gamesense at: {_gamesenseEndpoint}");
                await RegisterGameAsync();
                await RegisterEventAsync();
                _isRegistered = true;
                System.Diagnostics.Debug.WriteLine("Gamesense registration completed");
            }

            await SendEventAsync(mediaInfo);
        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine($"HTTP error connecting to Gamesense at {_gamesenseEndpoint}: {httpEx.Message}");
            _isRegistered = false; // Reset registration status to retry next time
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

        System.Diagnostics.Debug.WriteLine($"Registering game: {json}");
        var response = await _httpClient.PostAsync($"{_gamesenseEndpoint}/game_metadata", content);
        System.Diagnostics.Debug.WriteLine($"Game registration response: {response.StatusCode}");
    }

private async Task RegisterEventAsync()
{
    var eventData = new JObject
    {
        ["game"] = _gameId,
        ["event"] = _eventId,
        ["icon_id"] = 23,
        ["handlers"] = new JArray
        {
            new JObject
            {
                ["device-type"] = "screened",
                ["zone"] = "one",
                ["mode"] = "screen",
                ["datas"] = new JArray
                {
                    new JObject
                    {
                        ["lines"] = new JArray
                        {
                            new JObject
                            {
                                ["has-text"] = true,
                                ["context-frame-key"] = "song"
                            },
                            new JObject
                            {
                                ["has-text"] = true,
                                ["context-frame-key"] = "artist"
                            }
                        }
                    }
                }
            }
        }
    };

    var json = eventData.ToString(Formatting.None);
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
            value = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            frame = new Dictionary<string, object>
            {
                ["song"] = mediaInfo.Title,
                ["artist"] = mediaInfo.Artist
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
