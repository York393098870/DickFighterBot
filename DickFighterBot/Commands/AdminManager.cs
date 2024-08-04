using DickFighterBot.config;
using DickFighterBot.DataBase;
using DickFighterBot.PublicAPI;

namespace DickFighterBot.Commands;

public class AdminManager
{
    //管理员命令执行器

    public async Task 维护补偿(long user_id, long group_id, Message.GroupMessage message)
    {
        //维护补偿

        var senderId = message.user_id;

        if (senderId == ConfigLoader.Load().Management.Administrator)
        {
            var energyCompensate = 240;

            var dickFighterDataBase = new DickFighterDataBase();
            var compensationResult = await dickFighterDataBase.Compensation(group_id, energyCompensate);

            if (compensationResult)
                await WebSocketClient.Send(GroupMessageGenerator.Generate($"管理员已经为当前群玩家补偿{energyCompensate}点体力！",
                    group_id));
        }
    }
}