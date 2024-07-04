using CoreLibrary.PublicAPI;

namespace DickFighterBot.Functions;

public class ShowFunctions
{
    public static async Task ShowHelp(long group_id)
    {
        var helpMessage = "牛子系统指令列表：\n" +
                          "牛子系统正在升级，敬请期待！第一时间了解详情请加QQ群：745297798！\n" +
                          "1. 锻炼牛子X次：消耗体力锻炼牛子，可能增加或者减少长度\n" +
                          "3. 斗牛：消耗体力进行群内牛子PK，可能增加或者减少长度\n" +
                          "4. 改牛子名 [新名字]：修改牛子的名字\n" +
                          "5. 我的牛子：查询自己的牛子信息\n" +
                          "6. 牛子卡池：查看当前牛子卡池信息\n" +
                          "7.群牛子榜/全服牛子榜：查看群内/全服牛子榜单\n";
        await WebSocketClient.SendMessage(SendGroupMessage.Generate(helpMessage, group_id));
    }
}