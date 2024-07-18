using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;
using CoreLibrary.Tools;

namespace DickFighterBot.Functions;

public class 真理牛子
{
    //真理牛子可以将随机一只牛子的长度取对数，有效抑制膨胀

    public static async Task Logarithm(long group_id, long user_id, int energyCost = 80)
    {
        string outputMessage;

        var successRateList = new[] { 0.4, 0.6, 0.8, 1.0 };
        var successRate = successRateList[Random.Shared.Next(0, successRateList.Length)];
        var success = Random.Shared.NextDouble() < successRate;
        var dickFighterDataBase = new DickFighterDataBase();

        var (dickExisted, newDick) = await dickFighterDataBase.CheckDickWithIds(user_id, group_id);

        if (dickExisted)
        {
            var currentEnergy = newDick.Energy = await dickFighterDataBase.CheckDickEnergyWithGuid(newDick.GUID);

            if (currentEnergy >= energyCost)
            {
                newDick.Energy = currentEnergy;
                await dickFighterDataBase.UpdateDickEnergy(guid: newDick.GUID, energy:
                    currentEnergy - energyCost);
                if (success)
                {
                    //随机抓出一只牛子，取对数
                    var enemyDick = await dickFighterDataBase.GetRandomDick(newDick.GUID);
                    var enemyOldLength = enemyDick.Length;
                    double newLength;
                    if (enemyOldLength > 0)
                    {
                        newLength = Math.Log(enemyOldLength * 0.99 + 1) + 0.01 * enemyOldLength;
                    }
                    else
                    {
                        newLength = -Math.Log(Math.Abs(enemyOldLength * 0.99) + 1) + 0.01 * enemyOldLength;
                    }

                    var lengthDifference = newLength - enemyOldLength;
                    enemyDick.Length = newLength;
                    await dickFighterDataBase.UpdateDickLength(newLength, enemyDick.GUID);

                    var winnerGet = -lengthDifference * RandomGenerator.GetRandomDouble(0.2, 0.3);
                    newDick.Length += winnerGet;
                    await dickFighterDataBase.UpdateDickLength(newDick.Length, newDick.GUID);

                    outputMessage =
                        $"[CQ:at,qq={user_id}]，你花费{energyCost}体力，尝试使用试用牛子“真理牛子”对全服的随机牛子发动追加攻击！" +
                        $"根据星际牛子公司测算，本次追加攻击发动的概率为{successRate * 100}%。天有不测风云，牛有旦夕祸福。{enemyDick.Belongings}的牛子[{enemyDick.NickName}]受到了你的攻击!该牛子的长度从{enemyOldLength:F2}cm变化为{newLength:F2}cm，长度变化为{lengthDifference:F2}cm。" +
                        $"在追加攻击发动的同时，你的牛子[{newDick.NickName}]掠夺了{winnerGet:F2}cm的长度，当前长度为{newDick.Length:F2}cm。";
                }
                else
                {
                    outputMessage =
                        $"[CQ:at,qq={user_id}]，你花费{energyCost}体力，尝试使用试用牛子“真理牛子”对全服的随机牛子发动追加攻击！" +
                        $"根据星际牛子公司测算，本次追加攻击发动的概率为{successRate * 100}%。天有不测风云，牛有旦夕祸福。然而，你的牛子的追加攻击并没有生效，全服没有任何牛子发生了变化，仅仅是你损失了一些体力而已。";
                }
            }
            else
            {
                outputMessage = TipsMessage.EnergyNotEnough(currentEnergy, energyCost, user_id);
            }
        }
        else
        {
            outputMessage = TipsMessage.DickNotFound(user_id);
        }

        await WebSocketClient.Send(GroupMessageGenerator.Generate(outputMessage, group_id));
    }

    public static async Task 追加攻击(long group_id, long user_id, double succeedRate = 0.25)
    {
        //发动不消耗体力的追加攻击
        string outputMessage;

        var successRateList = new[] { 0.4, 0.6, 0.8, 1.0 };
        var successRate = successRateList[Random.Shared.Next(0, successRateList.Length)];
        var success = Random.Shared.NextDouble() < successRate;
        var dickFighterDataBase = new DickFighterDataBase();

        var (dickExisted, newDick) = await dickFighterDataBase.CheckDickWithIds(user_id, group_id);

        if (dickExisted)
        {
            if (success && Random.Shared.NextDouble() < succeedRate)
            {
                //随机抓出一只牛子，取对数
                var enemyDick = await dickFighterDataBase.GetRandomDick(newDick.GUID);
                var enemyOldLength = enemyDick.Length;
                double newLength;

                var pctForCalculate = 0.8d; //取对数的比例，只有这一部分会被取对数

                if (enemyOldLength > 0)
                {
                    newLength = Math.Log(enemyOldLength * pctForCalculate + 1) + (1 - pctForCalculate) * enemyOldLength;
                }
                else
                {
                    newLength = -Math.Log(Math.Abs(enemyOldLength * pctForCalculate) + 1) +
                                (1 - pctForCalculate) * enemyOldLength;
                }

                var lengthDifference = newLength - enemyOldLength;
                enemyDick.Length = newLength;
                await dickFighterDataBase.UpdateDickLength(newLength, enemyDick.GUID);

                var winnerGet = -lengthDifference * RandomGenerator.GetRandomDouble(0.2, 0.3);
                newDick.Length += winnerGet;
                await dickFighterDataBase.UpdateDickLength(newDick.Length, newDick.GUID);

                outputMessage =
                    $"[CQ:at,qq={user_id}]，你遇到了真理牛子，在真理牛子的帮助下，你对全服的随机牛子发动了弱化版本的追加攻击！" +
                    $"天有不测风云，牛有旦夕祸福。{enemyDick.Belongings}的牛子[{enemyDick.NickName}]受到了你的攻击!该牛子的长度从{enemyOldLength:F2}cm变化为{newLength:F2}cm，长度变化为{lengthDifference:F2}cm。" +
                    $"在追加攻击发动的同时，你的牛子[{newDick.NickName}]掠夺了{winnerGet:F2}cm的长度，当前长度为{newDick.Length:F2}cm。";
                await WebSocketClient.Send(GroupMessageGenerator.Generate(outputMessage, group_id));
            }
        }
        else
        {
            outputMessage = TipsMessage.DickNotFound(user_id);
            await WebSocketClient.Send(GroupMessageGenerator.Generate(outputMessage, group_id));
        }
    }
}