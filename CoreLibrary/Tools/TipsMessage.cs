namespace CoreLibrary.Tools;

public class TipsMessage
{
    public static string DickNotFound(long user_id)
    {
        return $"用户[CQ:at,qq={user_id}]，你压根就没有牛子，发什么√8指令！请使用“生成牛子”指令，生成一只牛子。";
    }
}