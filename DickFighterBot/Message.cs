namespace DickFighterBot;

public class Message
{
    // This class is used to deserialize the JSON message sent by the WebSocket server.
    public class GroupMessage
    {
        public long time { get; set; }
        public string? post_type { get; set; }
        public string? message_type { get; set; }
        public int message_id { get; set; }
        public long group_id { get; set; }
        public long user_id { get; set; }
        public dynamic message { get; set; }
        public string? raw_message { get; set; }
    }
}