using System.Text.Json;
using DickFighterBot.PublicAPI;

namespace DickFighterBot.Commands;

public class 语音生成器
{
    private async Task<string> Get(string message = "摸摸你的唧唧")
    {
        var requestUri = $"https://api.yujn.cn/api/yuyin.php?type=json&from=丁真&msg={Uri.EscapeDataString(message)}";

        using var client = new HttpClient();
        try
        {
            // 发送 GET 请求
            var response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            // 读取返回的 JSON 数据
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(jsonResponse);

            // 提取 URL
            if (jsonDoc.RootElement.TryGetProperty("url", out var urlElement))
            {
                var audioUrl = urlElement.GetString();

                // 下载音频文件
                var audioBytes = await client.GetByteArrayAsync(audioUrl);

                // 转换为 Base64 编码
                var base64Audio = Convert.ToBase64String(audioBytes);

                return base64Audio;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        throw new Exception("反正就是没能成功获取丁真的语句");
    }

    public async Task Send(long groupId, string message)
    {
        var base64Audio = await Get(message);
        await WebSocketClient.Send(群消息序列化工具.Generate(GenerateAudioMessage(base64Audio), groupId));
    }

    private string GenerateAudioMessage(string base64Audio)
    {
        var outputMessage = $"[CQ:record,file=base64://{base64Audio}]";
        return outputMessage;
    }
}