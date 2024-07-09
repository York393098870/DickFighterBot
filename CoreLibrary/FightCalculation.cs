using CoreLibrary.Tools;
using NLog;

namespace CoreLibrary;

public class FightCalculation
{
    //牛子对战的结果取决于以下几点：长度差（长度差越长，长的一方胜利的概率就越大），士气（士气高的一方胜利的概率就越大）
    //士气取决于最近三场战斗的胜负情况，胜利则士气+1，失败则士气-1
    //以上均为完整代码的情况，我想到了一个绝妙的办法，可以让功能更加完善，但是这里空白太小了，写不下

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static (bool isWin, double challengerChange, double defenderChange, double winRatePct) Calculate(
        double challengerLength,
        double defenderLength, int morale, double differenceValue, int times = 1)
    {
        var differenceValueRate = Mapping.NewMapping(differenceValue);
        var winRate = 1 / 6d + 1 / 3d * (differenceValueRate + 1);
        var winRatePct = winRate * 100;

        var boolResult = GenerateRandom.GetRandomDouble() < winRate; //判定胜负

        //核心算法部分
        if (boolResult)
        {
            Logger.Info("发起了一场随机牛子对决！挑战方获得了胜利！");
            var challengerChangeWhenWin = Math.Abs(differenceValue) * GenerateRandom.GetRandomDouble(0.1, 0.2) +
                                          GenerateRandom.GetRandomDouble(10, 20);
            var defenderChangeWhenLose = -Math.Abs(differenceValue) * GenerateRandom.GetRandomDouble(0.2, 0.3) -
                                         GenerateRandom.GetRandomDouble(5, 10);
            return (isWin: boolResult, challengerChange: times * challengerChangeWhenWin,
                defenderChange: times * defenderChangeWhenLose, winRatePct);
        }

        Logger.Info("发起了一场随机牛子对决！挑战方挑战失败！！");
        var challengerChangeWhenLose = -Math.Abs(differenceValue) * GenerateRandom.GetRandomDouble(0.15, 0.25) -
                                       GenerateRandom.GetRandomDouble(5, 10);
        var defenderChangeWhenWin = Math.Abs(differenceValue) * GenerateRandom.GetRandomDouble(0.05, 0.15) +
                                    GenerateRandom.GetRandomDouble(10, 20);

        return (isWin: boolResult, challengerChange: times * challengerChangeWhenLose,
            defenderChange: times * defenderChangeWhenWin, winRatePct);
    }
}