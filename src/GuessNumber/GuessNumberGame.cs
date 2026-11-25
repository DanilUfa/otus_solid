namespace GuessNumber;

public sealed class GuessNumberGame(
    ISecretNumberGenerator generator,
    IGameInput input,
    IGameOutput output)
{
    public void Play(GameSettings settings)
    {
        int secret = generator.Generate(settings.MinNumber, settings.MaxNumber);
        output.WriteLine($"Угадайте число от {settings.MinNumber} до {settings.MaxNumber} включительно.");
        output.WriteLine($"Количество попыток: {settings.MaxAttempts}.");

        int attemptsUsed = 0;
        while (attemptsUsed < settings.MaxAttempts)
        {
            output.WriteLine($"Попытка {attemptsUsed + 1}. Введите число:");
            string? line = input.ReadLine();
            if (line is null)
            {
                output.WriteLine("Ввод завершён. Игра остановлена.");
                return;
            }

            if (!int.TryParse(line, out int guess))
            {
                output.WriteLine("Нужно ввести целое число. Попытка не потрачена.");
                continue;
            }

            if (guess < settings.MinNumber || guess > settings.MaxNumber)
            {
                output.WriteLine("Число вне заданного диапазона. Попытка не потрачена.");
                continue;
            }

            attemptsUsed++;
            if (guess == secret)
            {
                output.WriteLine($"Угадали! Использовано попыток: {attemptsUsed}.");
                return;
            }

            output.WriteLine(guess < secret ? "Загаданное число больше." : "Загаданное число меньше.");
        }

        output.WriteLine($"Попытки закончились. Загаданное число: {secret}.");
    }
}
