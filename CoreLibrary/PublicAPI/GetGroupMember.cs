namespace CoreLibrary.SendMessages;

public class GetGroupMember
{
    public static void Main(long group_id)
    {
        var messageObject = new
        {
            action = "get_group_member_list_rate_limited",
            @params = new
            {
                group_id
            }
        };
    }
}