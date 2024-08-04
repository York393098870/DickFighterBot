using System.Diagnostics;
using CoreLibrary.PublicAPI;

namespace DickFighterBot.Commands;

public class StatusChecker
{
    public async Task Show(long user_id, long group_id, Message.GroupMessage message)
    {
        var uptime =
            DateTime.Now - Process.GetCurrentProcess().StartTime;
        var days = uptime.Days;
        var hours = uptime.Hours;
        var minutes = uptime.Minutes;
        var seconds = uptime.Seconds;
        var outputMessage = $"牛子系统正在运行！\n已运行时间：{days}天{hours}小时{minutes}分钟{seconds}秒";
        await WebSocketClient.Send(GroupMessageGenerator.Generate(outputMessage, group_id));
    }
}