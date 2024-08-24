using System.Text.Json;

namespace DickFighterBot.PublicAPI;

public static class 群消息序列化工具
{
    public static string Generate(string message, long groupId)
    {
        var messageObject = new
        {
            action = "send_group_msg_rate_limited",
            @params = new
            {
                group_id = groupId, message = message
            },
            echo = "客户端发送消息"
        };
        return JsonSerializer.Serialize(messageObject);
    }
    
}