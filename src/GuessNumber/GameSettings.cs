namespace GuessNumber;

public sealed class GameSettings
{
    public int MinNumber { get; }
    public int MaxNumber { get; }
    public int MaxAttempts { get; }

    public GameSettings(int minNumber, int maxNumber, int maxAttempts)
    {
        if (minNumber > maxNumber)
        {
            throw new ArgumentException("Минимальное число не может быть больше максимального.");
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAttempts);
        MinNumber = minNumber;
        MaxNumber = maxNumber;
        MaxAttempts = maxAttempts;
    }
}
