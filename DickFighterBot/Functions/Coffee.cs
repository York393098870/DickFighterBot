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
        var energyAdd = 60;
        var dickFighterDataBase = new DickFighterDataBase();
        var (dickExisted, dick) = await dickFighterDataBase.CheckDickWithTwoId(userId: user_id, groupId: group_id);

        if (dickExisted)
        {
            var checkResult = await dickFighterDataBase.CheckIfCoffeeLine(dick.GUID);
            if (!checkResult.Item1)
            {
                //还没创建记录，先创建一条
                await dickFighterDataBase.CreateNewCoffeeLine(dick.GUID);
                dick.Energy += energyAdd;
                await dickFighterDataBase.UpdateDickEnergy(dick.Energy, dick.GUID);

                outputMessage =
                    $"用户[CQ:at,qq={user_id}]，你的牛子{dick.NickName}饮用了一杯牛子咖啡，现在精神饱满，体力回复了{energyAdd}点。当前体力为{dick.Energy}/240。";
            }
            else
            {
                //存在记录，判定和上一次喝咖啡是否间隔超过一天（只判定天数，不是判定间隔24小时）
                var lastDrinkTime = DateTimeOffset.FromUnixTimeSeconds(checkResult.Item2);
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                var daysDifference = (DateTimeOffset.Now - lastDrinkTime).Days;
                if (daysDifference >= 1)
                {
                    //可以喝咖啡
                    dick.Energy += energyAdd;
                    await dickFighterDataBase.UpdateDickEnergy(dick.Energy, dick.GUID);

                    outputMessage =
                        $"用户[CQ:at,qq={user_id}]，你的牛子{dick.NickName}饮用了一杯牛子咖啡，现在精神饱满，体力回复了{energyAdd}点。当前体力为{dick.Energy}/240。";
                }
                else
                {
                    //还没到一天！
                    var timeNow = DateTimeOffset.Now;
                    var newDayTime =
                        new DateTimeOffset(timeNow.Year, timeNow.Month, timeNow.Day, 0, 0, 0, timeNow.Offset)
                            .AddDays(1);
                    var restOfTime = newDayTime - timeNow;
                    outputMessage =
                        $"用户[CQ:at,qq={user_id}]，你的牛子{dick.NickName}今天已经饮用过一杯咖啡了，请{restOfTime.Hours}小时{restOfTime.Minutes}分钟后再来！";
                }
            }

            await dickFighterDataBase.DrinkCoffee(dick.GUID);
        }
        else
        {
            outputMessage = TipsMessage.DickNotFound(user_id);
        }

        await WebSocketClient.SendMessage(SendGroupMessage.Generate(outputMessage, group_id));
    }
}