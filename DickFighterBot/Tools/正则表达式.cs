using System.Text.RegularExpressions;
using NLog;

namespace DickFighterBot.Tools;

public class 正则表达式
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static (bool ifNeed, long output) 咬(string input)
    {
        const string pattern = @"咬\[CQ:at,qq=(\d+)\]";
        var match = Regex.Match(input, pattern);

        if (!match.Success) return (false, 0);

        var result = Convert.ToInt64(match.Groups[1].Value);
        return (true, result);
    }
    public static (bool ifNeed, long output) 摸(string input)
    {
        const string pattern = @"摸\[CQ:at,qq=(\d+)\]";
        var match = Regex.Match(input, pattern);

        if (!match.Success) return (false, 0);

        var result = Convert.ToInt64(match.Groups[1].Value);
        return (true, result);
    }

    public static (string? newName, bool ifNeedEdit) 改牛子名(string input)
    {
        const string pattern = @"^改牛子名\s*(.*)$";

        // 使用正则表达式匹配输入字符串
        var match = Regex.Match(input, pattern);

        // 检查匹配结果
        if (match.Success && match.Groups[1].Value.Length <= 30)
            return (newName: match.Groups[1].Value, ifNeedEdit: true);

        return (newName: null, ifNeedEdit: false);
    }

    public static (bool ifNeed, string output) 丁真(string input)
    {
        const string pattern = @"^丁真\s*(.*)$";

        var match = Regex.Match(input, pattern);

        if (match.Success && match.Groups[1].Value.Length <= 10)
        {
            return (ifNeed: true, output: match.Groups[1].Value);
        }
        else
        {
            return (ifNeed: false, output: "");
        }
    }

    public static (int? exerciseTimes, bool ifNeedExercise) 锻炼牛子(string input)
    {
        const string pattern = @"^锻炼牛子(\d+)次$";

        // 使用正则表达式匹配输入字符串
        var match = Regex.Match(input, pattern);

        // 检查匹配结果
        if (!match.Success) return (exerciseTimes: null, ifNeedExercise: false);

        var exerciseTimesAvailable = int.TryParse(match.Groups[1].Value, out var result);

        if (exerciseTimesAvailable && result is > 0 and <= 241) return (exerciseTimes: result, ifNeedExercise: true);

        Logger.Warn("不合法的锻炼次数！");
        return (exerciseTimes: null, ifNeedExercise: false);
    }
}