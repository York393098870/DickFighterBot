namespace DickFighterBot.Tools;

public class RandomGenerator
{
    public static double GetRandomDouble(double min = 0d, double max = 1d)
    {
        var result = Random.Shared.NextDouble() * (max - min) + min;
        return result;
    }

    public static int GetRandomInt(int min, int max)
    {
        var result = Random.Shared.Next(min, max);
        return result;
    }
}