using GuessNumber;

namespace GuessNumber.Checks;

// Другая стратегия выбора числа: середина диапазона вместо случайного числа.
// Поддерживает весь контракт, в том числе отрицательные числа и крайние значения int.
internal sealed class MidpointSecretNumberGenerator : ISecretNumberGenerator
{
    public int Generate(int minNumber, int maxNumber)
    {
        if (minNumber > maxNumber)
        {
            throw new ArgumentException("Неверный диапазон чисел.");
        }

        return (int)(minNumber + ((long)maxNumber - minNumber) / 2);
    }
}

internal sealed class ScriptedInput(params string[] lines) : IGameInput
{
    private readonly Queue<string> _lines = new(lines);

    public int LinesRead { get; private set; }

    public string? ReadLine()
    {
        LinesRead++;
        return _lines.TryDequeue(out string? line) ? line : null;
    }
}

internal sealed class RecordingOutput : IGameOutput
{
    public List<string> Messages { get; } = [];

    public void WriteLine(string message) => Messages.Add(message);
}
