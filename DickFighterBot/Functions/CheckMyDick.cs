using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;
using CoreLibrary.Tools;

namespace DickFighterBot.Functions;

public class CheckMyDick
{
    public static async Task Main(long user_id, long group_id)
    {
        string stringMessage;
        var dickFighterDataBase = new DickFighterDataBase();

        //查询是否已经存在牛子
        var (dickExisted, newDick) =
            await dickFighterDataBase.CheckDickWithTwoId(user_id,
                group_id);
        if (dickExisted)
        {
            newDick.Energy = await dickFighterDataBase.CheckDickEnergyWithGuid(newDick.GUID);

            //计算排名
            var ranks = await dickFighterDataBase.GetLengthRanks(newDick.GUID, group_id);

            stringMessage = $"基本信息：\n" +
                            $"用户：[CQ:at,qq={user_id}]，你的牛子“{newDick.NickName}”，目前长度为{newDick.Length:F1}cm，当前体力状况：[{newDick.Energy}/240]\n" +
                            "排名信息：\n" +
                            $"牛子群内排名：[{ranks.groupRank}/{ranks.groupTotal}]名；牛子全服排名：[{ranks.globalRank}/{ranks.globalTotal}]名";
        }
        else
        {
            stringMessage = TipsMessage.DickNotFound(user_id);
        }

        await WebSocketClient.SendMessage(SendGroupMessage.Generate(stringMessage, group_id));
    }
}