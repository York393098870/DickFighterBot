using System.Text.RegularExpressions;

namespace CoreLibrary.Tools;

public class 正则表达式
{
    public static (string? newName, bool ifNeedEdit) 改牛子名(string input)
    {
        // 定义正则表达式模式
        const string pattern = @"^改牛子名\s*(.*)$";

        // 使用正则表达式匹配输入字符串
        var match = Regex.Match(input, pattern);

        // 检查匹配结果
        if (match.Success)
            // 提取括号内的内容并返回
            return (newName: match.Groups[1].Value, ifNeedEdit: true);

        // 如果不匹配，则不返回任何内容
        return (newName: null, ifNeedEdit: false);
    }
}