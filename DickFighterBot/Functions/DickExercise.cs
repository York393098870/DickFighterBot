using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;
using CoreLibrary.Tools;

namespace DickFighterBot.Functions;

public class DickExercise
{
    private static readonly int exercisePerCost = 30;

    public static async Task TryExercise(long user_id, long group_id, int exerciseTimes = 1)
    {
        string outputMessage;

        string[] winString =
        [
            "你的锻炼卓有成效，可惜并没有什么奇妙的事件发生，锻炼使你的牛子", "在锻炼完毕以后，你遇到了琴。她为你的牛子演奏了一首美妙的曲子，曲音如泉水般流淌，你的牛子在音乐的震动中有所感悟，",
            "在锻炼完毕以后，诺艾尔出现在你面前。她拿出她的石锤，为你的牛子进行了一次特制的石锤按摩，你的牛子在按摩过程中",
            "在锻炼的过程当中，银狼发现了你的牛子，她使用黑客技术为你的牛子进行了以太编码，这使得你的牛子", "在锻炼之前，莫娜发现了你的牛子，她运用占星术为你的牛子祈祷，这使得你的牛子"
        ];
        string[] loseString =
        [
            "过度锻炼使你的牛子受到损伤", "在锻炼过程当中，带着镰刀路过的希儿不小心损伤了你的牛子。疼痛使你的牛子瞬间", "在锻炼当中，你遇到了莫娜。她误以为你的牛子是一种妖怪，用魔法攻击了它，这使得你的牛子",
            "在石锤按摩的过程中，诺艾尔不小心用石锤换成了铁锤。这使得你的牛子", "在锻炼过程当中，你的牛子被一只蚊子叮了一口，这使得你的牛子"
        ];

        //查询是否已经存在牛子
        var (dickExisted, newDick) =
            await DickFighterDataBase.CheckPersonalDick(user_id,
                group_id);
        if (dickExisted)
        {
            var energyCost = exercisePerCost * exerciseTimes;
            //检查体力值是否足够
            newDick.Energy = await DickFighterDataBase.CheckEnergy(newDick.GUID);
            var currentEnergy = newDick.Energy;

            var tipsMessage = $"你可以使用指令“锻炼牛子X次”来快速锻炼牛子，每次锻炼消耗{exercisePerCost}体力值。";

            if (currentEnergy >= energyCost)
            {
                var newEnergy = currentEnergy - energyCost;
                await DickFighterDataBase.UpdateDickEnergy(guid: newDick.GUID, energy: newEnergy);
                var lengthDifference = GenerateRandom.GetRandomDouble(-5, 10) * exerciseTimes;
                newDick.Length += lengthDifference;
                await DickFighterDataBase.UpdateDickLength(newDick.Length, newDick.GUID);


                if (lengthDifference > 0)
                {
                    var winMessagePart1 = winString[Random.Shared.Next(0, winString.Length)];
                    outputMessage =
                        $"用户[CQ:at,qq={user_id}]，你的牛子“{newDick.NickName}”消耗{energyCost}体力值完成了{exerciseTimes}次锻炼！" +
                        winMessagePart1 +
                        $"增长了{Math.Abs(lengthDifference):F3}cm，你的牛子目前长度为{newDick.Length:F2}cm，体力值为{newEnergy}/240。" +
                        tipsMessage;
                }
                else
                {
                    var loseMessagePart1 = loseString[Random.Shared.Next(0, loseString.Length)];
                    outputMessage =
                        $"用户[CQ:at,qq={user_id}]，你的牛子“{newDick.NickName}”消耗{energyCost}体力值完成了{exerciseTimes}次锻炼！" +
                        loseMessagePart1 +
                        $"缩短了{Math.Abs(lengthDifference):F3}cm，你的牛子目前长度为{newDick.Length:F2}cm，体力值为{newEnergy}/240。" +
                        tipsMessage;
                }
            }
            else
            {
                outputMessage =
                    $"用户[CQ:at,qq={user_id}]，你的牛子“{newDick.NickName}”，体力值不足，无法锻炼！当前体力值为{currentEnergy}/240" +
                    tipsMessage;
            }
        }
        else
        {
            outputMessage = TipsMessage.DickNotFound(user_id);
        }

        await WebSocketClient.SendMessage(SendGroupMessage.Generate(outputMessage, group_id));
    }

    public static async Task IfNeedExercise(string rawMessage, long user_id, long group_id)
    {
        var input = 正则表达式.锻炼牛子(rawMessage);
        if (input.ifNeedExercise)
            await TryExercise(user_id, group_id,
                Convert.ToInt32(input.exerciseTimes));
    }
}