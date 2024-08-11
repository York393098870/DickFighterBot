using DickFighterBot.config;
using DickFighterBot.DataBase;
using DickFighterBot.PublicAPI;
using NLog;

namespace DickFighterBot.Commands;

public class AdminManager
{
    //管理员命令执行器
    
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public async Task 维护补偿(long group_id, Message.GroupMessage message)
    {
        //维护补偿

        var senderId = message.user_id;

        if (senderId == ConfigLoader.Load().Management.Administrator)
        {
            var energyCompensate = 240;

            var dickFighterDataBase = new DickFighterDataBase();
            var compensationResult = await dickFighterDataBase.Compensation(group_id, energyCompensate);

            if (compensationResult)
            {
                await WebSocketClient.Send(GroupMessageGenerator.Generate($"管理员已经为当前群玩家补偿{energyCompensate}点体力！",
                    group_id));
                Logger.Info($"管理员{senderId}为群{group_id}的玩家补偿了{energyCompensate}点体力！");
            }
                
        }
    }
}