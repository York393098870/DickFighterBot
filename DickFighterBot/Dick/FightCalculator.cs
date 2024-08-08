using DickFighterBot.Tools;

namespace DickFighterBot.Dick;

public class FightCalculator
{
    //牛子对战的结果取决于以下几点：长度差（长度差越长，长的一方胜利的概率就越大）

    public static (bool isWin, double challengerChange, double defenderChange, double winRatePct) Calculate(
        double challengerLength,
        double defenderLength, double differenceValue)
    {
        var differenceValueRate = Mapping.NewMapping(differenceValue);

        //增加一个随机因素
        var randomRate = RandomGenerator.GetRandomDouble() * 0.25d;

        var winRate = randomRate + 75 / 100d / 2 * (differenceValueRate + 1);
        var winRatePct = winRate * 100;

        var isWin = RandomGenerator.GetRandomDouble() < winRate; //判定胜负

        var absDifferenceValue = Math.Abs(differenceValue);

        double[] numberList =
        [
            absDifferenceValue * RandomGenerator.GetRandomDouble(0.1, 0.3) +
            RandomGenerator.GetRandomDouble(10, 20),
            2 * (Math.Abs(challengerLength) * 0.9 + RandomGenerator.GetRandomDouble(10, 15)),
            2 * (Math.Abs(defenderLength) * 0.95 + RandomGenerator.GetRandomDouble(5, 20))
        ];

        //核心算法部分
        if (isWin)
        {
            var defenderChangeWhenLose = -numberList.Min();
            var challengerChangeWhenWin = Math.Abs(defenderChangeWhenLose) * RandomGenerator.GetRandomDouble(0.2, 0.3);
            return (isWin, challengerChangeWhenWin, defenderChangeWhenLose, winRatePct);
        }

        var challengerChangeWhenLose = -numberList.Min();
        var defenderChangeWhenWin = Math.Abs(challengerChangeWhenLose) * RandomGenerator.GetRandomDouble(0.2, 0.3);

        return (isWin, challengerChangeWhenLose, defenderChangeWhenWin, winRatePct);
    }
}