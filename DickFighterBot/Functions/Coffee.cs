using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;
using CoreLibrary.Tools;
using NLog;

namespace DickFighterBot.Functions;

public class Coffee
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static async Task DrinkCoffee(long user_id, long group_id)
    {
        string outputMessage;
        const int energyAdd = 60;
        var dickFighterDataBase = new DickFighterDataBase();
        var (dickExisted, dick) = await dickFighterDataBase.CheckDickWithIds(user_id, group_id);

        if (dickExisted)
        {
            var (recordExisted, lastDrinkTimeFromDataBase) = await dickFighterDataBase.CheckIfCoffeeLine(dick.GUID);

            dick.Energy = await dickFighterDataBase.CheckDickEnergyWithGuid(dick.GUID);

            switch (recordExisted)
            {
                case false:
                    //数据库当中没有对应的记录，先创建一条初始记录

                    await dickFighterDataBase.CreateNewCoffeeLine(dick.GUID);
                    dick.Energy += energyAdd;
                    await dickFighterDataBase.UpdateDickEnergy(dick.Energy, dick.GUID);

                    Logger.Info($"群{group_id}的用户{user_id}没有咖啡记录，新增一条咖啡记录！");

                    outputMessage =
                        $"[CQ:at,qq={user_id}]，你的牛子[{dick.NickName}]饮用了一杯牛子咖啡，现在精神饱满，体力回复了{energyAdd}点。当前体力为{dick.Energy}/240。";
                    break;

                case true:
                {
                    //存在相关记录
                    var lastDrinkTime = DateTimeOffset.FromUnixTimeSeconds(lastDrinkTimeFromDataBase);
                    var nextAvailableTime = lastDrinkTime.AddHours(20);
                    var currentTime = DateTimeOffset.Now;

                    if (nextAvailableTime < currentTime) //判断上次喝咖啡的时间和现在是否超过了22小时
                    {
                        //可以喝咖啡
                        await dickFighterDataBase.DrinkCoffee(dick.GUID);
                        dick.Energy += energyAdd;
                        await dickFighterDataBase.UpdateDickEnergy(dick.Energy, dick.GUID);

                        Logger.Info($"群{group_id}的用户{user_id}饮用了一杯牛子咖啡。");

                        outputMessage =
                            $"[CQ:at,qq={user_id}]，你的牛子[{dick.NickName}]饮用了一杯牛子咖啡，现在精神饱满，体力回复了{energyAdd}点。当前体力为{dick.Energy}/240。";
                    }
                    else
                    {
                        var restOfTime = nextAvailableTime - currentTime;
                        outputMessage =
                            $"[CQ:at,qq={user_id}]，你的牛子[{dick.NickName}]今天已经饮用过一杯咖啡了，请{restOfTime.Hours}小时{restOfTime.Minutes}分钟后再来！";
                    }

                    break;
                }
            }
        }
        else
        {
            outputMessage = TipsMessage.DickNotFound(user_id);
        }

        await WebSocketClient.Send(SendGroupMessage.Generate(outputMessage, group_id));
    }
}