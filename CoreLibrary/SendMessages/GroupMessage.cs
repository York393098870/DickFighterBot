using System.Text.Json;

namespace CoreLibrary.SendMessages;

public class GroupMessage
{
    public static string SendGroupMessage(string message, long groupId)
    {
        var messageObject = new
        {
            action = "send_group_msg_rate_limited",
            @params = new
            {
                groupId, message
            }
        };
        var result = JsonSerializer.Serialize(messageObject);
        return result;
    }
}