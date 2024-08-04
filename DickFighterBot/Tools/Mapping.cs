namespace DickFighterBot.Tools;

public class Mapping
{
    //根据给定的值，映射返回一个[-1,1]之间的值，参数a控制前半段的增长速度，参数b控制后半段的增长速度

    public static double NewMapping(double value, double a = 0.001, double b = 0.125)
    {
        double result;

        if (value >= 0)
            result = Math.Log(1 + a * value) / (b + Math.Log(1 + a * value));
        else
            result = -Math.Log(1 - a * value) / (b + Math.Log(1 - a * value));

        return result;
    }
}