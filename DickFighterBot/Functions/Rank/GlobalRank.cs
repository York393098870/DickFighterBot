using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;
using NLog;

namespace DickFighterBot.Functions.Rank;

public class GlobalRank
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static async Task GetRank(long group_id, int n = 5)
    {
        string outputMessage;

        var dataBase = new DickFighterDataBase();

        var count = await dataBase.GetCountOfTotalDicks(); //获取数据库中牛子的总数，以确认要显示几个牛子

        if (count <= 0)
        {
            outputMessage = "数据库中没有足够的牛子，无法生成排名！";
            Logger.Error("数据库中没有足够的牛子，无法生成排名！");
        }
        else
        {
            //由于数据库中的牛子往往非常的多，在此，我们只会显示前n个牛子的排名，假如n小于数据库中的牛子总数，显示全部牛子的排名

            var dickCount = Math.Min(count, n); //与其使用条件运算符，不如使用Math.Min()函数

            var dickList = await dataBase.GetFirstNDicksByOrder(dickCount); //获取前n个牛子

            outputMessage = "当前排名如下：\n全服最长牛子榜\n排名|昵称|长度\n";

            for (var i = 1; i <= dickList.Count; i++)
            {
                var rankMessage =
                    $"{i}. 牛子昵称：{dickList[i].NickName}，长度：{dickList[i].Length:F1}cm，主人QQ:{dickList[i].Belongings}\n";
                outputMessage += rankMessage;
            }

            Logger.Info("全服最长牛子榜单已生成！");
        }

        await WebSocketClient.SendMessage(SendGroupMessage.Generate(outputMessage, group_id));
    }
}