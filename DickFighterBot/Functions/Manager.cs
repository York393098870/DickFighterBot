using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;

namespace DickFighterBot.Functions;

public class Manager
{
    //提供管理员功能
    public static async Task EnergyAdd(long groupId, int energyAdd = 60)
    {
        var dataBase = new DickFighterDataBase();
        await dataBase.CompensationOnEnergy(groupId, energyAdd);
        var message = $"管理员已经为群{groupId}的玩家补充{energyAdd}点体力！";
        await WebSocketClient.Send(GroupMessageGenerator.Generate(message, groupId));
    }
}