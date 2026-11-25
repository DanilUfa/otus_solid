namespace GuessNumber;

public sealed class ConsoleGameInput : IGameInput
{
    public string? ReadLine() => Console.ReadLine();
}
