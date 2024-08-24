using NLog;

namespace DickFighterBot.PublicAPI;

public class TryGetBase64
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static async Task<string> GetBase64(string url)
    {
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
            Logger.Error($"获取图片时出现问题: {ex.Message}");
            return null;
        }
    }

    public static async Task<string> GetWithQQ(string url,long qq)
    {
        using var client = new HttpClient();

        var targetUrl=url+$"?QQ={qq}";
        //示例：https://api.lolimi.cn/API/face_suck/api.php?QQ=393098870
        Console.WriteLine("构建的目标URL "+targetUrl);

        try
        {
            // 发送 GET 请求并获取响应的字节数组
            var imgData = await client.GetByteArrayAsync(targetUrl);

            // 将字节数组转换为 Base64 字符串
            var base64String = Convert.ToBase64String(imgData);

            // 返回 Base64 字符串
            return base64String;
        }
        catch (Exception ex)
        {
            Logger.Error($"获取图片时出现问题: {ex.Message}");
            return null;
        }
    }
}