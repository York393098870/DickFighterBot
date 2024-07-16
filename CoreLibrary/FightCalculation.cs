using CoreLibrary.Tools;
using NLog;

namespace CoreLibrary;

public class FightCalculation
{
    //牛子对战的结果取决于以下几点：长度差（长度差越长，长的一方胜利的概率就越大）

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static (bool isWin, double challengerChange, double defenderChange, double winRatePct) Calculate(
        double challengerLength,
        double defenderLength, int morale, double differenceValue, int times = 1)
    {
        var differenceValueRate = Mapping.NewMapping(differenceValue);

        //增加一个随机因素
        var randomRate = RandomGenerator.GetRandomDouble() * 0.2d;

        var winRate = randomRate + (8 / 10d) / 2 * (differenceValueRate + 1);
        var winRatePct = winRate * 100;

        var boolResult = RandomGenerator.GetRandomDouble() < winRate; //判定胜负

        //核心算法部分
        if (boolResult)
        {
            Logger.Info("发起了一场随机牛子对决！挑战方获得了胜利！");
            var challengerChangeWhenWin = Math.Abs(differenceValue) * RandomGenerator.GetRandomDouble(0.1, 0.2) +
                                          RandomGenerator.GetRandomDouble(10, 20);
            var defenderChangeWhenLose = -Math.Abs(differenceValue) * RandomGenerator.GetRandomDouble(0.2, 0.3) -
                                         RandomGenerator.GetRandomDouble(5, 10);
            return (isWin: boolResult, challengerChange: times * challengerChangeWhenWin,
                defenderChange: times * defenderChangeWhenLose, winRatePct);
        }

        Logger.Info("发起了一场随机牛子对决！挑战方挑战失败！！");
        var challengerChangeWhenLose = -Math.Abs(differenceValue) * RandomGenerator.GetRandomDouble(0.15, 0.25) -
                                       RandomGenerator.GetRandomDouble(5, 10);
        var defenderChangeWhenWin = Math.Abs(differenceValue) * RandomGenerator.GetRandomDouble(0.05, 0.15) +
                                    RandomGenerator.GetRandomDouble(10, 20);

        return (isWin: boolResult, challengerChange: times * challengerChangeWhenLose,
            defenderChange: times * defenderChangeWhenWin, winRatePct);
    }
}