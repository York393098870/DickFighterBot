using CoreLibrary.PublicAPI;
using NLog;

namespace DickFighterBot.Functions.DickGacha;

public class GachaPool
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static async Task Show(long group_id)
    {
        //展示当前的卡池信息
        var message0 = "卡池目前开发中，功能尚不完善，仅供参考：\n当前的卡池信息如下：\n";
        var message1 = "1. 五星限定牛子：\n[黄泉牛子]\n技能：我为牛子哀哭，消耗两倍斗牛体力，同时对五只牛子发动AOE攻击";
        var message2 = "2. 五星常驻牛子：\n[开拓者牛子]\n技能：常驻的牛子有个√8的技能，老老实实抽限定牛子去";
        var message3 = "3. 卡池概率公示：\n五星牛子概率：1.2%\n获得限定牛子概率为50%，若上一次没能获得限定牛子，则下一次必定获得限定牛子\n";
        var outputMessage = message0 + message1 + message2 + message3;
        await WebSocketClient.SendMessage(SendGroupMessage.Generate(outputMessage, group_id));
    }
}