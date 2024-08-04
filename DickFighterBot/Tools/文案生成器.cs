namespace DickFighterBot.Tools;

public class 文案生成器
{
    public static string WinWhenFight(long challengerId, string challengerNickName, long enemyId, string enemyNickName)
    {
        string[] stringList =
        [
            $"[CQ:at,qq={challengerId}]的牛子[{challengerNickName}]在一声怒吼中，奋力一击，将{enemyId}的牛子[{enemyNickName}]击倒在地！[{challengerNickName}]赢得了本次斗牛的胜利！",
            $"[CQ:at,qq={challengerId}]的牛子[{challengerNickName}]如闪电一般，迅猛出击，令{enemyId}的牛子[{enemyNickName}]措手不及，最终赢得了比赛！这是一场毫无悬念的胜利！",
            $"[CQ:at,qq={challengerId}]的牛子[{challengerNickName}]在一阵风的助力下突然间加速，猛然撞击{enemyId}的牛子[{enemyNickName}]，取得了意外的胜利！"
        ];
        return stringList[Random.Shared.Next(0, stringList.Length)];
    }

    public static string LoseWhenFight(long belongings, string? nickName, long enemyDickBelongings,
        string? enemyDickNickName)
    {
        string[] stringList =
        [
            $"[CQ:at,qq={belongings}]的牛子[{nickName}]在一声惨叫中，被{enemyDickBelongings}的牛子[{enemyDickNickName}]击倒在地！[{nickName}]最终输掉了本局比赛！",
            $"[CQ:at,qq={belongings}]的牛子[{nickName}]在一次失误中，被{enemyDickBelongings}的牛子[{enemyDickNickName}]抓住机会予以痛击，最终输掉了比赛！",
            $"[CQ:at,qq={belongings}]的牛子[{nickName}]在一次疏忽中，被{enemyDickBelongings}的牛子[{enemyDickNickName}]抓住机会，最终输掉了比赛！"
        ];
        return stringList[Random.Shared.Next(0, stringList.Length)];
    }
}