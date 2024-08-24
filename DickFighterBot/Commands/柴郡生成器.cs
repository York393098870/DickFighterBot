using DickFighterBot.PublicAPI;
using NLog;

namespace DickFighterBot.Commands;

public class 柴郡生成器
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static int 缓存数量 = 2;
    private static string?[] pictures = new string?[缓存数量];

    private static async Task 缓存图片()
    {
        Logger.Trace("开始缓存柴郡图片！");
        for (var i = 0; i < 缓存数量; i++)
        {
            if (pictures[i] == null || pictures[i] == "")
            {
                pictures[i] = await TryGetBase64();
            }
        }

        Logger.Trace("柴郡图片缓存完成！");
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
        await 缓存图片();
        for (var i = 0; i < 缓存数量; i++)
        {
            await WebSocketClient.Send(群消息序列化工具.Generate(GeneratePicMessage(pictures[i]), GroupId));
            pictures[i] = null;
        }

        Logger.Trace("发送图片完成，正在重新缓存！");
        await 缓存图片();
    }

    private string GeneratePicMessage(string base64Pic)
    {
        var outputMessage = $"[CQ:image,file=base64://{base64Pic}]";
        return outputMessage;
    }
}