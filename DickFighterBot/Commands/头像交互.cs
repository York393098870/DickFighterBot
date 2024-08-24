using DickFighterBot.PublicAPI;
using DickFighterBot.PublicAPI.Message;
using NLog;

namespace DickFighterBot.Commands;

public class 头像交互
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public async Task 咬(long groupId, long targetQQ)
    {
        var targetPic = await TryGetBase64.GetWithQQ("https://api.lolimi.cn/API/face_suck/api.php", targetQQ);
        var message = PicMessage.GeneratePicMessage(targetPic);
        await WebSocketClient.Send(群消息序列化工具.Generate(message, groupId));
    }

    public async Task 摸(long groupId, long targetQQ)
    {
        var targetPic = await TryGetBase64.GetWithQQ("https://api.lolimi.cn/API/face_petpet/api.php", targetQQ);
        var message = PicMessage.GeneratePicMessage(targetPic);
        await WebSocketClient.Send(群消息序列化工具.Generate(message, groupId));
    }
}