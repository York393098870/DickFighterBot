using CoreLibrary.DataBase;
using CoreLibrary.SendMessages;
using CoreLibrary.Tools;

namespace DickFighterBot.Functions;

public class ChangeDickName
{
    public static async Task Main(long user_id, long group_id, string rawMessage)
    {
        var (newName, ifNeedEdit) = 正则表达式.改牛子名(rawMessage);
        var (item1, newDick) =
            await DickFighterDataBase.CheckPersonalDick(user_id,
                group_id);

        if (ifNeedEdit)
        {
            string stringMessage;
            if (item1)
            {
                //如果需要修改名字并且有牛子
                stringMessage = $"[CQ:at,qq={user_id}]，你的牛子名字已经修改为[{newName}]！";
                await DickFighterDataBase.UpdateDickNickName(user_id, group_id, newName);
            }
            else
            {
                stringMessage = $"[CQ:at,qq={user_id}]，你还没有牛子！请使用“生成牛子”指令，生成一只牛子。";
            }

            await WebSocketClient.SendMessage(SendGroupMessage.Generate(stringMessage, group_id));
        }
    }
}