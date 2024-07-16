using CoreLibrary.PublicAPI;

namespace DickFighterBot.Functions;

public class ShowFunctions
{
    public static async Task ShowHelp(long group_id)
    {
        var helpMessage = "牛子系统指令列表：\n" +
                          "牛子系统正在升级，敬请期待！第一时间了解详情请加QQ群：745297798！\n" +
                          "1. 锻炼牛子X次：消耗体力锻炼牛子，可能增加或者减少长度\n" +
                          "2. 斗牛：消耗体力进行跨服牛子PK，可能增加或者减少长度\n" +
                          "3. 群内斗牛：消耗更多体力，进行更加紧张刺激的群内随机匹配斗牛\n" +
                          "4. 改牛子名 [新名字]：修改牛子的名字\n" +
                          "5. 我的牛子：查询自己的牛子信息\n" +
                          "6. 群牛子榜/全服牛子榜：查看群内/全服牛子榜单\n" +
                          "7. 牛子咖啡：饮用一杯咖啡，回复一定的体力\n" +
                          "8. 真理牛子：一个临时试用牛子，花费大量体力对随机牛子发动追加攻击，追加攻击一旦成功，对方牛子将会被取对数，自己也会获得一部分收益。\n";
        await WebSocketClient.Send(GroupMessageGenerator.Generate(helpMessage, group_id));
    }
}