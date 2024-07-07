using System.Diagnostics;
using CoreLibrary.PublicAPI;

namespace DickFighterBot.Functions;

public class CurrentStatus
{
    // This is a simple function that returns the current status of the bot.
    public static async Task Main(long group_id)
    {
        var uptime =
            DateTime.Now - Process.GetCurrentProcess().StartTime;
        var days = uptime.Days;
        var hours = uptime.Hours;
        var minutes = uptime.Minutes;
        var seconds = uptime.Seconds;
        var message = $"牛子系统正在运行！\n已运行时间：{days}天{hours}小时{minutes}分钟{seconds}秒";
        await WebSocketClient.SendMessage(SendGroupMessage.Generate(message, group_id));
    }
}