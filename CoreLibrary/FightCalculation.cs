﻿using CoreLibrary.Tools;

namespace CoreLibrary;

public class FightCalculation
{
    //牛子对战的结果取决于以下几点：长度差（长度差越长，长的一方胜利的概率就越大），士气（士气高的一方胜利的概率就越大）
    //士气取决于最近三场战斗的胜负情况，胜利则士气+1，失败则士气-1
    //士气太低的牛子遭受攻击时，会触发Buff“背水一战”，胜利概率大幅度提升！
    public static (bool isWin, double challengerChange, double defenderChange) Calculate(double challengerLength,
        double defenderLength, int morale, double differenceValue,
        double maxRange = 100d)
    {
        var differenceValueRate = Mapping.LogarithmicMapping(differenceValue, maxRange);
        var winRate = 1 / 3d + 1 / 6d * (differenceValueRate + 1);

        var boolResult = GenerateRandom.GetRandomDouble() < winRate; //随机生成一个0-1之间的数，判定胜负

        if (boolResult)
        {
            //挑战成功的逻辑

            Console.WriteLine("获得了胜利！");
            var challengerChangeWhenWin = Math.Abs(differenceValueRate) * GenerateRandom.GetRandomDouble(0.2, 0.4) +
                                          GenerateRandom.GetRandomDouble(0, 10);
            var defenderChangeWhenLose = -Math.Abs(differenceValueRate) * GenerateRandom.GetRandomDouble(0.1, 0.5) -
                                         GenerateRandom.GetRandomDouble(0, 10);
            return (isWin: boolResult, challengerChange: challengerChangeWhenWin,
                defenderChange: defenderChangeWhenLose);
        }

        Console.WriteLine("很遗憾失败了！");
        var challengerChangeWhenLose = -Math.Abs(differenceValueRate) * GenerateRandom.GetRandomDouble(0.2, 0.4) -
                                       GenerateRandom.GetRandomDouble(0, 10);
        var defenderChangeWhenWin = Math.Abs(differenceValueRate) * GenerateRandom.GetRandomDouble(0.1, 0.5) +
                                    GenerateRandom.GetRandomDouble(0, 10);

        return (isWin: boolResult, challengerChange: challengerChangeWhenLose,
            defenderChange: defenderChangeWhenWin);
    }
}