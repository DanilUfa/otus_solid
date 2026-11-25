using System.Text.Json;

namespace GuessNumber;

public sealed class JsonGameSettingsProvider(string path) : IGameSettingsProvider
{
    public GameSettings Load()
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<GameSettings>(json)
            ?? throw new InvalidDataException("Файл настроек не должен содержать null.");
    }
}
