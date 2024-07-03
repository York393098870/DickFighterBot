using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;
using CoreLibrary.Tools;
using NLog;

namespace DickFighterBot.Functions;

public class ChangeDickName
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static async Task Main(long user_id, long group_id, string rawMessage)
    {
        var (newName, ifNeedEdit) = 正则表达式.改牛子名(rawMessage);
        var (dickExisted, newDick) =
            await DickFighterDataBase.CheckPersonalDick(user_id,
                group_id);

        if (ifNeedEdit)
        {
            string stringMessage;
            if (dickExisted)
            {
                //如果需要修改名字并且有牛子
                var changeResult = await DickFighterDataBase.UpdateDickNickName(user_id, group_id, newName);
                if (changeResult)
                {
                    stringMessage = $"用户[CQ:at,qq={user_id}]，你的牛子名字已经修改为[{newName}]！";
                    Logger.Info($"群{group_id}当中的用户{user_id}修改了牛子昵称，新昵称为：{newName}");
                }
                else
                {
                    stringMessage = $"用户[CQ:at,qq={user_id}]，你的牛子名字修改失败！请稍后再试！";
                    Logger.Error($"群{group_id}当中的用户{user_id}修改牛子昵称失败！");
                }
            }
            else
            {
                stringMessage = TipsMessage.DickNotFound(user_id);
            }

            await WebSocketClient.SendMessage(SendGroupMessage.Generate(stringMessage, group_id));
        }
    }
}