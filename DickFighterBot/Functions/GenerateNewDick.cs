using CoreLibrary;
using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;
using CoreLibrary.Tools;

namespace DickFighterBot.Functions;

public class GenerateNewDick
{
    public static async Task Main(long user_id, long group_id)
    {
        //判断是否已经有了牛子
        var checkResult =
            await DickFighterDataBase.CheckDickWithTwoId(user_id,
                group_id);
        var ifExist = checkResult.Item1;

        string stringMessage;

        if (ifExist)
        {
            stringMessage = $"用户[CQ:at,qq={user_id}]，你已经有了一只牛子，请不要贪心！";
        }
        else
        {
            var newGuid = Guid.NewGuid().ToString();
            var newDick = new Dick(user_id, "无名牛子", 0,
                GenerateRandom.GetRandomDouble(5d, 15d), newGuid);

            await DickFighterDataBase.GenerateNewDick(user_id, group_id,
                newDick);

            stringMessage =
                $"用户[CQ:at,qq={user_id}]，你的牛子[{newDick.GUID}]已经成功生成，初始长度为{newDick.Length:F3}cm。\n" +
                $"初始生成的牛子默认拥有240点体力，请及时使用，防止体力溢出！你可以使用“改牛子名 [新牛子名]”指令来更改牛子的姓名。";
        }

        await WebSocketClient.SendMessage(SendGroupMessage.Generate(stringMessage, group_id));
    }
}