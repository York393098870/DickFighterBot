using DickFighterBot.Commands;
using DickFighterBot.Functions;
using Coffee = DickFighterBot.Commands.Coffee;
using DickChecker = DickFighterBot.Commands.DickChecker;
using DickFighter = DickFighterBot.Commands.DickFighter;

namespace DickFighterBot;

public class CommandDispatcher
{
    //命令调度器，根据收到的消息内容，分发到对应的命令执行器

    private readonly Dictionary<string, Func<long, long, Message.GroupMessage, Task>> _commands = new();

    public CommandDispatcher()
    {
        //构造函数，注册命令
        _commands.Add("/status",
            (userId, groupId, groupMessage) => new StatusChecker().Show(userId, groupId, groupMessage));
        _commands.Add("牛子帮助", (userId, groupId, groupMessage) => new Helper().ShowHelp(userId, groupId, groupMessage));
        _commands.Add("我的牛子",
            (userId, groupId, groupMessage) => new DickChecker().CheckSelfDick(userId, groupId, groupMessage));
        _commands.Add("生成牛子",
            (userId, groupId, groupMessage) => new DickGenerator().Generate(userId, groupId, groupMessage));
        _commands.Add("斗牛", (userId, groupId, _) => new DickFighter().NewFight(userId, groupId));
        _commands.Add("群牛子榜",
            (_, groupId, _) => new DickRank().GetGroupRank(groupId));
        _commands.Add("全服牛子榜",
            (userId, groupId, groupMessage) => new DickRank().GetGlobalRank(userId, groupId, groupMessage));
        _commands.Add("真理牛子",
            (userId, groupId, _) => new TruthDick().追加攻击(userId, groupId));
        _commands.Add("牛子咖啡", (userId, groupId, _) => new Coffee().DrinkCoffee(userId, groupId));
        _commands.Add("补偿体力",
            (_, groupId, groupMessage) => new AdminManager().维护补偿(groupId, groupMessage));
        /*_commands.Add("说脏话", (_, groupId, _) => new 语音生成器().Send(groupId));*/
        _commands.Add("柴郡", (_, groupId, _) => new 柴郡生成器().Generate(groupId));
        _commands.Add("不好看的", (_, groupId, _) => new 好看的().Generate(groupId));
    }

    public async Task Dispatch(long user_id, long group_id, Message.GroupMessage message)
    {
        //分发命令
        var rawMessage = message.raw_message;
        if (rawMessage != null && _commands.TryGetValue(rawMessage, out var command))
        {
            //执行命令
            await command(user_id, group_id, message);
        }
        else if (rawMessage != null)
        {
            //处理非常规命令，例如含有参数的命令
            if (rawMessage.Contains("锻炼牛子"))
                await DickExercise.IfNeedExercise(rawMessage, user_id, group_id);
            else if (rawMessage.Contains("改牛子名")) await DickNameChanger.Change(user_id, group_id, rawMessage);
            else if (rawMessage.Contains("丁真"))
            {
                var 内容 = Tools.正则表达式.丁真(rawMessage);
                if (内容.ifNeed)
                {
                    var 语音生成器 = new 语音生成器();
                    await 语音生成器.Send(group_id, 内容.output);
                }
            }
            else if (rawMessage.Contains("咬"))
            {
                var targetQQ = Tools.正则表达式.咬(rawMessage);
                if (targetQQ.ifNeed)
                {
                    var 头像交互 = new 头像交互();
                    await 头像交互.咬(group_id, targetQQ.output);
                }
            }else if (rawMessage.Contains("摸"))
            {
                var targetQQ = Tools.正则表达式.摸(rawMessage);
                if (targetQQ.ifNeed)
                {
                    var 头像交互 = new 头像交互();
                    await 头像交互.摸(group_id, targetQQ.output);
                }
            }
        }
    }
}