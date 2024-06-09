namespace DickFighterBot;

public class Message
{
    public class GroupMessage
    {
        public long time { get; set; }
        public string? post_type { get; set; }
        public string? message_type { get; set; }
        public int message_id { get; set; }
        public long group_id { get; set; }
        public long user_id { get; set; }
        public string? message { get; set; }
        public string? raw_message { get; set; }
    }
}