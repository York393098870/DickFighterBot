using DickFighterBot.PublicAPI;
using NLog;

namespace DickFighterBot.Commands;

public class 好看的
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    private static async Task<string?> TryGetBase64()
    {
        const string url = "http://api.yujn.cn/api/yht.php?type=image";

        using var client = new HttpClient();
        try
        {
            // 发送 GET 请求并获取响应的字节数组
            var imgData = await client.GetByteArrayAsync(url);

            // 将字节数组转换为 Base64 字符串
            var base64String = Convert.ToBase64String(imgData);

            // 返回 Base64 字符串
            return base64String;
        }
        catch (Exception ex)
        {
            Logger.Error($"获取好看的图片时出现问题: {ex.Message}");
            return null;
        }
    }
    private string GeneratePicMessage(string base64Pic)
    {
        var outputMessage = $"[CQ:image,file=base64://{base64Pic}]";
        return outputMessage;
    }
    public async Task Generate(long GroupId)
    {
        var base64Pic = await TryGetBase64();
        await WebSocketClient.Send(群消息序列化工具.Generate(GeneratePicMessage(base64Pic), GroupId));
    }
}