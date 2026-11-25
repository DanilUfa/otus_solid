namespace GuessNumber;

public interface IGameSettingsProvider
{
    GameSettings Load();
}

public interface ISecretNumberGenerator
{
    /// <summary>Возвращает число в диапазоне [minNumber, maxNumber], включая обе границы.</summary>
    /// <remarks>Предусловие: minNumber не больше maxNumber. Допустим диапазон из одного числа.</remarks>
    int Generate(int minNumber, int maxNumber);
}

public interface IGameInput
{
    /// <summary>Читает строку. null означает завершение ввода.</summary>
    string? ReadLine();
}

public interface IGameOutput
{
    void WriteLine(string message);
}
