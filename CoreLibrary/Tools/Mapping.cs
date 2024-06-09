namespace CoreLibrary.Tools;

public class Mapping
{
    //根据给定的值，映射返回一个[-1,1]之间的值，K值为陡峭程度

    public static double SigmoidMapping(double value, double maxRange, double k = 2d)
    {
        if (value >= maxRange)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be less than range.");
        }
        
        if (value < 0)
        {
            return -1.0 / (1.0 + Math.Exp(k * (value / maxRange + 0.5)));
        }

        return 1.0 / (1.0 + Math.Exp(-k * (value / maxRange - 0.5)));
    }

    public static double LogarithmicMapping(double value, double maxRange, double k = 0.5d)
    {
        if (value > maxRange)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and M.");
        }

        if (value < 0)
        {
            return Math.Log(-value * k + 1) / Math.Log(maxRange * k + 1);
        }

        return value == 0 ? 0 : Math.Log(value * k + 1) / Math.Log(maxRange * k + 1);
    }
}