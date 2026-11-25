namespace GuessNumber;

public sealed class RandomSecretNumberGenerator : ISecretNumberGenerator
{
    public int Generate(int minNumber, int maxNumber)
    {
        if (minNumber > maxNumber)
        {
            throw new ArgumentException("Неверный диапазон чисел.");
        }

        // long позволяет включить int.MaxValue в диапазон без переполнения.
        return (int)Random.Shared.NextInt64(minNumber, (long)maxNumber + 1);
    }
}
