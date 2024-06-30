using CoreLibrary;
using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;

namespace DickFighterBot.Functions;

public class 斗牛
{
    public static async Task Main(long user_id, long group_id)
    {
        string stringMessage;
        var (item1, challengerDick) =
            await DickFighterDataBase.CheckPersonalDick(user_id,
                group_id);
        if (item1)
        {
            challengerDick.Energy = await DickFighterDataBase.CheckEnergy(challengerDick.GUID);
            var currentEnergy = challengerDick.Energy;
            if (currentEnergy >= 40)
            {
                //体力充足，扣取体力以后决斗
                challengerDick.Energy -= 40;
                await DickFighterDataBase.UpdateDickEnergy(challengerDick.Energy,
                    challengerDick.GUID);

                //查询群内其他人的牛子，随机选择一只牛子进行对战
                var defenderDick = await DickFighterDataBase.GetRandomDick(group_id,
                    challengerDick.GUID); //防止自己打自己
                if (defenderDick != null)
                {
                    var battleResult = FightCalculation.Calculate(challengerDick.Length,
                        defenderDick.Length, 0, challengerDick.Length - defenderDick.Length);
                    var stringMessage1 =
                        $"用户[CQ:at,qq={user_id}]，你的牛子“{challengerDick.NickName}”，消耗20点体力，向 {defenderDick.Belongings}的牛子“{defenderDick.NickName}” 发起了斗牛！根据牛科院物理研究所计算，你的牛子胜率为{battleResult.winRatePct:F1}%。";
                    string stringMessage2;

                    //更新牛子长度
                    challengerDick.Length += battleResult.challengerChange;
                    defenderDick.Length += battleResult.defenderChange;
                    await DickFighterDataBase.UpdateDickLength(challengerDick.Length,
                        challengerDick.GUID);
                    await DickFighterDataBase.UpdateDickLength(defenderDick.Length,
                        defenderDick.GUID);

                    if (battleResult.isWin)
                    {
                        stringMessage2 =
                            $"你的牛子“{challengerDick.NickName}”在斗牛当中获得了胜利！长度增加了{battleResult.challengerChange:F3}cm，目前长度为{challengerDick.Length:F1}cm。对方牛子“{defenderDick.NickName}”长度变化为{battleResult.defenderChange:F3}cm，目前长度为{defenderDick.Length:F1}cm。";
                    }
                    else
                    {
                        stringMessage2 =
                            $"你的牛子“{challengerDick.NickName}”在斗牛当中遗憾地失败！长度变化为{battleResult.challengerChange:F3}cm，目前长度为{challengerDick.Length:F1}cm。对方牛子“{defenderDick.NickName}”长度增加了{battleResult.defenderChange:F3}cm，目前长度为{defenderDick.Length:F1}cm";
                    }

                    stringMessage = stringMessage1 + stringMessage2;
                }
                else
                {
                    stringMessage = $"用户[CQ:at,qq={user_id}]，群内没有其他牛子！快邀请一只牛子进群吧！";
                }
            }
            else
            {
                stringMessage =
                    $"用户{user_id}，你的牛子“{challengerDick.NickName}”，体力值不足，无法斗牛！当前体力值为{currentEnergy}/240";
            }
        }
        else
        {
            stringMessage = $"用户[CQ:at,qq={user_id}]，你还没有牛子！请使用“生成牛子”指令，生成一只牛子。";
        }

        await WebSocketClient.SendMessage(SendGroupMessage.Generate(stringMessage, group_id));
    }
}