using CoreLibrary.PublicAPI;

namespace DickFighterBot.Functions;

public class Wife
{
    public static async Task Main(long group_id, long user_id)
    {
        string stringMessage;
        var randomNumber = Random.Shared.NextDouble() * 100;
        // stringMessage=$"[CQ:at,qq={user_id}]，你今日waifu是：。她的润滑度为：{randomNumber:F2}%，插入成功！";
        //await WebSocketClient.SendMessage(SendGroupMessage.Generate(stringMessage, group_id));
        await WebSocketClient.Send(GetGroupMember.Main(group_id));
    }
}