using System.Text.Json;

namespace CoreLibrary.PublicAPI;

public class GetGroupMember
{
    public static string Main(long group_id)
    {
        var messageObject = new
        {
            action = "get_group_member_list_rate_limited",
            @params = new
            {
                group_id
            }
        };
        return JsonSerializer.Serialize(messageObject);
    }
}