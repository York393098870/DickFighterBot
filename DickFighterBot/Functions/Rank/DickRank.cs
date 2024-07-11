using CoreLibrary.config;
using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;
using NLog;

namespace DickFighterBot.Functions.Rank;

public class DickRank
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static async Task GetGlobalRank(long group_id)
    {
        string outputMessage;

        var n = LoadConfig.Load().Rank.GlobalRankTopCount;

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
                    $"{i}. 牛子昵称：{dickList[i - 1].NickName}，长度：{dickList[i - 1].Length:F1}cm，主人QQ:{dickList[i - 1].Belongings}\n";
                outputMessage += rankMessage;
            }

            dickList = await dataBase.GetFirstNDicksByOrder(dickCount, 1); //获取倒数的牛子
            outputMessage += "\n全服最短牛子榜\n排名|昵称|长度\n";

            for (var i = 1; i <= dickList.Count; i++)
            {
                var rankMessage =
                    $"{i}. 牛子昵称：{dickList[i - 1].NickName}，长度：{dickList[i - 1].Length:F1}cm，主人QQ:{dickList[i - 1].Belongings}\n";
                outputMessage += rankMessage;
            }

            Logger.Info("全服牛子榜单已生成！");
        }

        await WebSocketClient.Send(SendGroupMessage.Generate(outputMessage, group_id));
    }

    public static async Task GetGroupRank(long group_id)
    {
        string outputMessage;

        var n = LoadConfig.Load().Rank.GroupRankTopCount;

        var dataBase = new DickFighterDataBase();

        var count = await dataBase.GetCountOfTotalDicks(); //获取数据库中牛子的总数，以确认要显示几个牛子

        if (count <= 0)
        {
            outputMessage = "当前群内没有足够的牛子，无法生成排名！";
            Logger.Error("当前群内没有足够的牛子，无法生成排名！");
        }
        else
        {
            //由于数据库中的牛子往往非常的多，在此，我们只会显示前n个牛子的排名，假如n小于数据库中的牛子总数，显示全部牛子的排名

            var dickCount = Math.Min(count, n); //与其使用条件运算符，不如使用Math.Min()函数

            var dickList = await dataBase.GetFirstNDicksByOrder(dickCount, group_id); //获取前n个牛子

            outputMessage = "当前排名如下：\n群最长牛子榜\n排名|昵称|长度\n";

            for (var i = 1; i <= dickList.Count; i++)
            {
                var rankMessage =
                    $"{i}. 牛子昵称：{dickList[i - 1].NickName}，长度：{dickList[i - 1].Length:F1}cm，主人QQ:{dickList[i - 1].Belongings}\n";
                outputMessage += rankMessage;
            }

            dickList = await dataBase.GetFirstNDicksByOrder(dickCount, order: 1, group_id: group_id); //获取倒数的牛子
            outputMessage += "\n群最短牛子榜\n排名|昵称|长度\n";

            for (var i = 1; i <= dickList.Count; i++)
            {
                var rankMessage =
                    $"{i}. 牛子昵称：{dickList[i - 1].NickName}，长度：{dickList[i - 1].Length:F1}cm，主人QQ:{dickList[i - 1].Belongings}\n";
                outputMessage += rankMessage;
            }

            Logger.Info($"群{group_id}牛子榜单已生成！");
        }

        await WebSocketClient.Send(SendGroupMessage.Generate(outputMessage, group_id));
    }
}