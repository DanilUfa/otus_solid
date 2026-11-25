namespace GuessNumber;

public sealed class ConsoleGameOutput : IGameOutput
{
    public void WriteLine(string message) => Console.WriteLine(message);
}
