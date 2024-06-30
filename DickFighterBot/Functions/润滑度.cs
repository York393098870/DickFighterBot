using CoreLibrary.PublicAPI;

namespace DickFighterBot.Functions;

public class 润滑度
{
    public static async Task Main(long user_id,long group_id)
    {
        var randomNumber = Random.Shared.NextDouble() * 100;
        var stringMessagePart1 =
            $"用户[CQ:at,qq={user_id}]，你今日的直肠润滑度为{randomNumber:F2}%，";
        var stringMessagePart2 = randomNumber switch
        {
            <= 25 => $"前进的道路上阻力并不小。",
            <= 50 => $"稍加准备便可以顺利进入。",
            <= 75 => $"预计进入的过程将会丝滑且流畅。",
            _ => $"看起来比较松弛，需要痔疮弥补这一部分。"
        };

        const string stringMessagePart3 = $"\n牢Rin寄语：";

        string[] stringList =
        [
            $"润滑剂也是非常有用的一种工具。", "直肠马上就打开了。", "怎么润滑都不过分。", "这使南通感到害怕了。", "食用辣椒也是提升体验的一种方式。", "真的不考虑加入一些能够提升摩擦力的物品吗？",
            "痔疮并不是一定要急于切除的。", "这不是勾引人鸿儒吗？", "你还没有向群主献出沟子。"
        ];
        var stringMessagePart4 = stringList[Random.Shared.Next(0, stringList.Length)];
        var stringMessage = stringMessagePart1 + stringMessagePart2 + stringMessagePart3 + stringMessagePart4;
        await WebSocketClient.SendMessage(SendGroupMessage.Generate(stringMessage, group_id));
    }
}