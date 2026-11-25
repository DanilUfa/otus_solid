using System.Text;
using System.Text.Json;
using GuessNumber;
using GuessNumber.Checks;

Console.OutputEncoding = Encoding.UTF8;

var checks = new (string Name, Action Run)[]
{
    ("Победа с последней попытки и обе подсказки", VerifyHintsAndWin),
    ("Поражение после заданного числа попыток", VerifyLoss),
    ("Неверный ввод не тратит попытки", VerifyInvalidInput),
    ("Завершение ввода останавливает игру", VerifyEndOfInput),
    ("Неверные настройки отклоняются", VerifyInvalidSettings),
    ("Настройки загружаются из JSON", VerifyJsonSettings),
    ("Контракт генераторов на разных диапазонах", VerifyGeneratorContract),
    ("Замена генератора без изменения игры", VerifyGeneratorSubstitution)
};

int failures = 0;
foreach (var (name, run) in checks)
{
    try
    {
        run();
        Console.WriteLine($"PASS: {name}");
    }
    catch (Exception error)
    {
        failures++;
        Console.Error.WriteLine($"FAIL: {name}: {error.Message}");
    }
}

Console.WriteLine($"Пройдено: {checks.Length - failures}/{checks.Length}");
return failures == 0 ? 0 : 1;

static void VerifyHintsAndWin()
{
    var output = new RecordingOutput();
    var input = new ScriptedInput("3", "7", "5");
    var game = new GuessNumberGame(new MidpointSecretNumberGenerator(), input, output);
    Check(game.Play(new GameSettings(1, 9, 3)) == GameResult.Won, "Ожидалась победа.");
    Check(output.Messages.Contains("Загаданное число больше."), "Нет подсказки «больше».");
    Check(output.Messages.Contains("Загаданное число меньше."), "Нет подсказки «меньше».");
    Check(input.LinesRead == 3, "Неверное число попыток.");
}

static void VerifyLoss()
{
    var input = new ScriptedInput("1", "2", "5");
    var output = new RecordingOutput();
    var game = new GuessNumberGame(new MidpointSecretNumberGenerator(), input, output);
    Check(game.Play(new GameSettings(1, 9, 2)) == GameResult.Lost, "Ожидалось поражение.");
    Check(input.LinesRead == 2, "Игра превысила лимит попыток.");
    Check(output.Messages.Last() == "Попытки закончились. Загаданное число: 5.", "Не показан ответ.");
}

static void VerifyInvalidInput()
{
    var input = new ScriptedInput("", "abc", "2147483648", "0", "10", "5");
    var game = new GuessNumberGame(new MidpointSecretNumberGenerator(), input, new RecordingOutput());
    Check(game.Play(new GameSettings(1, 9, 1)) == GameResult.Won, "Ошибки ввода потратили попытку.");
    Check(input.LinesRead == 6, "Обработаны не все строки.");
}

static void VerifyEndOfInput()
{
    var game = new GuessNumberGame(new MidpointSecretNumberGenerator(),
        new ScriptedInput(), new RecordingOutput());
    Check(game.Play(new GameSettings(1, 9, 3)) == GameResult.Cancelled, "Игра не остановлена.");
}

static void VerifyInvalidSettings()
{
    ExpectException<ArgumentException>(() => new GameSettings(10, 1, 3));
    ExpectException<ArgumentOutOfRangeException>(() => new GameSettings(1, 10, 0));
    ExpectException<ArgumentOutOfRangeException>(() => new GameSettings(1, 10, -1));
}

static void VerifyJsonSettings()
{
    string path = Path.GetTempFileName();
    try
    {
        File.WriteAllText(path, """{"MinNumber":-10,"MaxNumber":20,"MaxAttempts":4}""");
        IGameSettingsProvider provider = new JsonGameSettingsProvider(path);
        GameSettings settings = provider.Load();
        Check(settings.MinNumber == -10 && settings.MaxNumber == 20 && settings.MaxAttempts == 4,
            "Параметры не прочитаны из файла.");

        File.WriteAllText(path, "null");
        ExpectException<InvalidDataException>(() => provider.Load());
        File.WriteAllText(path, "{");
        ExpectException<JsonException>(() => provider.Load());
        File.WriteAllText(path, """{"MinNumber":10,"MaxNumber":1,"MaxAttempts":4}""");
        ExpectException<ArgumentException>(() => provider.Load());
    }
    finally
    {
        File.Delete(path);
    }
}

static void VerifyGeneratorContract()
{
    ISecretNumberGenerator[] generators =
        [new RandomSecretNumberGenerator(), new MidpointSecretNumberGenerator()];
    (int Min, int Max)[] ranges =
        [(1, 100), (-10, -1), (0, 0), (int.MinValue, int.MinValue),
         (int.MaxValue, int.MaxValue), (int.MinValue, int.MaxValue)];

    foreach (var generator in generators)
    {
        foreach (var (min, max) in ranges)
        {
            for (int i = 0; i < 100; i++)
            {
                int number = generator.Generate(min, max);
                Check(number >= min && number <= max, "Генератор нарушил границы диапазона.");
            }
        }

        ExpectException<ArgumentException>(() => generator.Generate(2, 1));
    }
}

static void VerifyGeneratorSubstitution()
{
    ISecretNumberGenerator[] generators =
        [new RandomSecretNumberGenerator(), new MidpointSecretNumberGenerator()];

    foreach (var generator in generators)
    {
        // Перебор гарантирует победу при любом результате корректного генератора.
        var game = new GuessNumberGame(generator, new ScriptedInput("-1", "0", "1"),
            new RecordingOutput());
        Check(game.Play(new GameSettings(-1, 1, 3)) == GameResult.Won,
            "После замены генератора игра перестала работать.");
    }
}

static void ExpectException<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException($"Ожидалось исключение {typeof(TException).Name}.");
}

static void Check(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
