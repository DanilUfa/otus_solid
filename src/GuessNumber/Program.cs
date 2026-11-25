using System.Text;
using System.Text.Json;
using GuessNumber;

Console.OutputEncoding = Encoding.UTF8;

try
{
    string settingsPath = Path.Combine(AppContext.BaseDirectory, "settings.json");

    IGameSettingsProvider settingsProvider = new JsonGameSettingsProvider(settingsPath);
    GameSettings settings = settingsProvider.Load();
    var game = new GuessNumberGame(
        new RandomSecretNumberGenerator(),
        new ConsoleGameInput(),
        new ConsoleGameOutput());

    game.Play(settings);
    return 0;
}
catch (Exception error) when (error is IOException or UnauthorizedAccessException
    or JsonException or ArgumentException)
{
    Console.Error.WriteLine($"Не удалось запустить игру: {error.Message}");
    return 1;
}
