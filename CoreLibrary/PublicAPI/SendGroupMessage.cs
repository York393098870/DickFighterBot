using System.Text.Json;

namespace CoreLibrary.SendMessages;

public class SendGroupMessage
{
    public static string Generate(string message, long groupId)
    {
        var messageObject = new
        {
            action = "send_group_msg_rate_limited",
            @params = new
            {
                group_id=groupId, message
            }
        };
        var result = JsonSerializer.Serialize(messageObject);
        return result;
    }
}