namespace CoreLibrary.Tools;

public class TipsMessage
{
    public static string DickNotFound(long user_id)
    {
        return $"[CQ:at,qq={user_id}]，你压根就没有牛子，发什么√8指令！请使用“生成牛子”指令，生成一只牛子。";
    }

    public static string EnergyNotEnough(int currentEnergy, int energyCost, long user_id)
    {
        return $"[CQ:at,qq={user_id}]，你的牛子体力值不足，无法进行操作！当前体力值为{currentEnergy}/240，本次操作需要消耗{energyCost}体力值。";
    }
}