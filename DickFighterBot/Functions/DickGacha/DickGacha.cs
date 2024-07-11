using CoreLibrary.DataBase;

namespace DickFighterBot.Functions.DickGacha;

public class DickGacha
{
    public async Task Main(long user_id, long group_id)
    {
        var dickFighterDataBase = new DickFighterDataBase();
        //首先判定是否有牛子
        var DickInfo = await dickFighterDataBase.CheckDickWithIds(user_id, group_id);
        string Message;
        if (DickInfo.ifExisted)
        {
            //牛子存在，检查抽卡道具（专票）是否足够
        }
        //生成牛子
    }
}