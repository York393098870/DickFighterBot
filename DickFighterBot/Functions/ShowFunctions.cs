using CoreLibrary.PublicAPI;

namespace DickFighterBot.Functions;

public class ShowFunctions
{
    public static async Task ShowHelp(long group_id)
    {
        var helpMessage = "牛子系统指令列表：\n" +
                          "牛子系统正在升级，敬请期待！第一时间了解详情请加QQ群：745297798！\n" +
                          "1. 生成牛子：生成一只牛子\n" +
                          "2. 锻炼牛子：消耗体力锻炼牛子，可能增加或者减少长度\n" +
                          "3. 斗牛：消耗体力进行群内牛子PK，可能增加或者减少长度\n" +
                          "4. 改牛子名 [新名字]：修改牛子的名字\n" +
                          "5. 润滑度：查询你的今日润滑度\n" +
                          "6. 我的牛子：查询自己的牛子信息\n";
        await WebSocketClient.SendMessage(SendGroupMessage.Generate(helpMessage, group_id));
    }
}