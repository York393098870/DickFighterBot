using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;
using CoreLibrary.Tools;

namespace DickFighterBot.Functions;

public class Exercise
{
    public static async Task Main(long user_id, long group_id)
    {
        string stringMessage;

        //查询是否已经存在牛子
        var (item1, newDick) =
            await DickFighterDataBase.CheckPersonalDick(user_id,
                group_id);
        if (item1)
        {
            //检查体力值
            newDick.Energy = await DickFighterDataBase.CheckEnergy(newDick.GUID);
            var currentEnergy = newDick.Energy;
            if (currentEnergy >= 40)
            {
                //体力值足够
                var newEnergy = currentEnergy - 40;
                await DickFighterDataBase.UpdateDickEnergy(guid: newDick.GUID, energy: newEnergy);
                var lengthDifference = GenerateRandom.GetRandomDouble(-5, 15);
                newDick.Length += lengthDifference;
                await DickFighterDataBase.UpdateDickLength(newDick.Length, newDick.GUID);
                stringMessage =
                    $"用户[CQ:at,qq={user_id}]，你的牛子“{newDick.NickName}”，锻炼成功！消耗40体力值，当前体力值为{newEnergy}/240，锻炼使得牛子长度变化{lengthDifference:F2}cm，目前牛子长度为{newDick.Length:F2}cm";
            }
            else
            {
                stringMessage =
                    $"用户[CQ:at,qq={user_id}]，你的牛子“{newDick.NickName}”，体力值不足，无法锻炼！当前体力值为{currentEnergy}/240";
            }
        }
        else
        {
            stringMessage = $"用户[CQ:at,qq={user_id}]，你还没有牛子！请使用“生成牛子”指令，生成一只牛子。";
        }

        await WebSocketClient.SendMessage(SendGroupMessage.Generate(stringMessage, group_id));
    }
}