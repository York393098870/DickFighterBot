using DickFighterBot.PublicAPI;
using DickFighterBot.Tools;

namespace DickFighterBot.Commands;

public class Helper
{
    public async Task ShowHelp(long user_id, long group_id, Message.GroupMessage message)
    {
        var helpMessage = "牛子系统指令列表：\n" +
                          "牛子系统正在升级，敬请期待！第一时间了解详情请加官方QQ群：745297798！\n" +
                          "1. 牛子帮助：展示帮助菜单\n" +
                          "2. 锻炼牛子X次：消耗体力锻炼牛子，可能增加或者减少长度\n" +
                          "3. 斗牛：消耗体力进行跨服牛子PK，可能增加或者减少长度\n" +
                          "4. 群内斗牛：消耗更多体力，进行更加紧张刺激的群内随机匹配斗牛\n" +
                          "5. 改牛子名 [新名字]：修改牛子的名字，太长会被驳回\n" +
                          "6. 我的牛子：查询自己的牛子信息\n" +
                          "7. 群牛子榜/全服牛子榜：查看群内/全服牛子榜单\n" +
                          "8. 牛子咖啡：饮用一杯咖啡，回复一定的体力,每20个小时可以饮用一次\n" +
                          "9. 真理牛子：一个临时试用牛子，花费大量体力对随机牛子发动追加攻击，\n追加攻击一旦成功，对方牛子将会被取对数，自己也会获得一部分收益。\n";
        var outputPhoto = TextToImageConverter.ConvertToBase64(helpMessage);
        var outputMessage = $"[CQ:image,file=base64://{outputPhoto}]";
        await WebSocketClient.Send(GroupMessageGenerator.Generate(outputMessage, group_id));
    }
}