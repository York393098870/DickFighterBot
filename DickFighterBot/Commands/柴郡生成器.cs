using DickFighterBot.PublicAPI;
using NLog;

namespace DickFighterBot.Commands;

public class 柴郡生成器
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static string?[] pictures = new string?[2];

    private static async Task 缓存图片()
    {
        Logger.Trace("开始缓存柴郡图片！");
        for (var i = 0; i < 2; i++)
        {
            pictures[i] = await TryGetBase64();
        }

        Logger.Trace($"柴郡图片缓存完成！");
    }

    private static async Task<string?> TryGetBase64()
    {
        const string url = "https://api.lolimi.cn/API/chaiq/c.php";

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
            Logger.Error($"获取柴郡图片时出现问题: {ex.Message}");
            return null;
        }
    }

    public async Task Generate(long GroupId)
    {
        if (pictures[0] == null || pictures[1] == null || pictures[0] == "" || pictures[1] == "")
        {
            Logger.Trace("柴郡图片缓存为空，正在尝试缓存新的柴郡图片！");
            await 缓存图片();
        }

        await WebSocketClient.Send(GroupMessageGenerator.Generate(GeneratePicMessage(pictures[0]), GroupId));
        await WebSocketClient.Send(GroupMessageGenerator.Generate(GeneratePicMessage(pictures[1]), GroupId));
        Logger.Trace("发送图片完成，正在重新缓存！");
        await 缓存图片();
    }

    private string GeneratePicMessage(string base64Pic)
    {
        var outputMessage = $"[CQ:image,file=base64://{base64Pic}]";
        return outputMessage;
    }
}