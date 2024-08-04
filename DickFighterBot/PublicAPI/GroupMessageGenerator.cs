using System.Text.Json;

namespace DickFighterBot.PublicAPI;

public static class GroupMessageGenerator
{
    public static string Generate(string message, long groupId)
    {
        var messageObject = new
        {
            action = "send_group_msg_rate_limited",
            @params = new
            {
                group_id = groupId, message
            }
        };
        return JsonSerializer.Serialize(messageObject);
    }
}