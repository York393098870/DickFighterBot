﻿using CoreLibrary;
using CoreLibrary.config;
using CoreLibrary.DataBase;
using CoreLibrary.PublicAPI;
using CoreLibrary.Tools;

#pragma warning disable CS8602 // 解引用可能出现空引用。

namespace DickFighterBot.Functions;

public class 斗牛
{
    public static async Task FightInGroup(long user_id, long group_id)
    {
        string outputMessage;

        var energyCost = LoadConfig.Load().DickData.FightEnergyCost;

        var dickFighterDataBase = new DickFighterDataBase();

        var (dickExisted, challengerDick) =
            await dickFighterDataBase.CheckDickWithIds(user_id,
                group_id);
        if (dickExisted)
        {
            challengerDick.Energy = await dickFighterDataBase.CheckDickEnergyWithGuid(challengerDick.GUID);

            var currentEnergy = challengerDick.Energy;
            if (currentEnergy >= energyCost)
            {
                //体力充足，扣取体力以后决斗
                challengerDick.Energy -= energyCost;
                await dickFighterDataBase.UpdateDickEnergy(challengerDick.Energy,
                    challengerDick.GUID);

                //查询群内其他人的牛子，随机选择一只牛子进行对战
                var defenderDick = await dickFighterDataBase.GetRandomDick(group_id,
                    challengerDick.GUID); //防止自己打自己
                if (defenderDick != null)
                {
                    var battleResult = FightCalculation.Calculate(challengerDick.Length,
                        defenderDick.Length, 0, challengerDick.Length - defenderDick.Length);
                    var stringMessage1 =
                        $"[CQ:at,qq={user_id}]，你的牛子“{challengerDick.NickName}”，消耗{energyCost}点体力，向 {defenderDick.Belongings}的牛子“{defenderDick.NickName}” 发起了斗牛！根据牛科院物理研究所计算，你的牛子胜率为{battleResult.winRatePct:F1}%。";

                    //更新牛子长度
                    challengerDick.Length += battleResult.challengerChange;
                    defenderDick.Length += battleResult.defenderChange;
                    await dickFighterDataBase.UpdateDickLength(challengerDick.Length,
                        challengerDick.GUID);
                    await dickFighterDataBase.UpdateDickLength(defenderDick.Length,
                        defenderDick.GUID);

                    var stringMessage2 = battleResult.isWin
                        ? $"可喜可贺的是，你的牛子“{challengerDick.NickName}”在斗牛当中获得了胜利！长度增加了{battleResult.challengerChange:F3}cm，目前长度为{challengerDick.Length:F1}cm。对方牛子“{defenderDick.NickName}”缩短了{Math.Abs(battleResult.defenderChange):F3}cm，目前长度为{defenderDick.Length:F1}cm。"
                        : $"不幸的是，你的牛子“{challengerDick.NickName}”在斗牛当中遗憾地失败！长度缩短了{Math.Abs(battleResult.challengerChange):F3}cm，目前长度为{challengerDick.Length:F1}cm。对方牛子“{defenderDick.NickName}”长度增加了{battleResult.defenderChange:F3}cm，目前长度为{defenderDick.Length:F1}cm";

                    var stringMessage3 = $"\n目前，你的牛子体力值为{challengerDick.Energy}/240。";

                    outputMessage = stringMessage1 + stringMessage2 + stringMessage3;
                }
                else
                {
                    outputMessage = $"[CQ:at,qq={user_id}]，群内没有其他牛子！快邀请一只牛子进群吧！";
                }
            }
            else
            {
                outputMessage =
                    $"[CQ:at,qq={user_id}]，你的牛子“{challengerDick.NickName}”，体力值不足，无法斗牛！当前体力值为{currentEnergy}/240";
            }
        }
        else
        {
            outputMessage = TipsMessage.DickNotFound(user_id);
        }

        await WebSocketClient.Send(SendGroupMessage.Generate(outputMessage, group_id));
    }

    public static async Task Fight(long user_id, long group_id)
    {
        string outputMessage;

        var energyCost = LoadConfig.Load().DickData.FightEnergyCost * 2;

        var dickFighterDataBase = new DickFighterDataBase();

        var (dickExisted, challengerDick) =
            await dickFighterDataBase.CheckDickWithIds(user_id,
                group_id);

        if (dickExisted)
        {
            challengerDick.Energy = await dickFighterDataBase.CheckDickEnergyWithGuid(challengerDick.GUID);

            var currentEnergy = challengerDick.Energy;
            if (currentEnergy >= energyCost)
            {
                //体力充足，扣取体力以后决斗
                challengerDick.Energy -= energyCost;
                await dickFighterDataBase.UpdateDickEnergy(challengerDick.Energy,
                    challengerDick.GUID);

                //查询其他人的牛子，随机选择一只牛子进行对战
                var defenderDick = await dickFighterDataBase.GetRandomDick(
                    challengerDick.GUID); //防止自己打自己

                if (defenderDick != null)
                {
                    var battleResult = FightCalculation.Calculate(challengerDick.Length,
                        defenderDick.Length, 0, challengerDick.Length - defenderDick.Length, 2);
                    var stringMessage1 =
                        $"[CQ:at,qq={user_id}]，你的牛子“{challengerDick.NickName}”，消耗{energyCost}点体力，向{defenderDick.Belongings}的牛子“{defenderDick.NickName}” 发起了跨服斗牛！根据牛科院物理研究所计算，你的牛子胜率为{battleResult.winRatePct:F1}%。";

                    //更新牛子长度
                    challengerDick.Length += battleResult.challengerChange;
                    defenderDick.Length += battleResult.defenderChange;
                    await dickFighterDataBase.UpdateDickLength(challengerDick.Length,
                        challengerDick.GUID);
                    await dickFighterDataBase.UpdateDickLength(defenderDick.Length,
                        defenderDick.GUID);

                    var stringMessage2 = battleResult.isWin
                        ? $"可喜可贺的是，你的牛子“{challengerDick.NickName}”在斗牛当中获得了胜利！长度增加了{battleResult.challengerChange:F3}cm，目前长度为{challengerDick.Length:F1}cm。对方牛子“{defenderDick.NickName}”缩短了{Math.Abs(battleResult.defenderChange):F3}cm，目前长度为{defenderDick.Length:F1}cm。"
                        : $"不幸的是，你的牛子“{challengerDick.NickName}”在斗牛当中遗憾地失败！长度缩短了{Math.Abs(battleResult.challengerChange):F3}cm，目前长度为{challengerDick.Length:F1}cm。对方牛子“{defenderDick.NickName}”长度增加了{battleResult.defenderChange:F3}cm，目前长度为{defenderDick.Length:F1}cm";

                    var stringMessage3 = $"\n目前，你的牛子体力值为{challengerDick.Energy}/240。";

                    outputMessage = stringMessage1 + stringMessage2 + stringMessage3;
                }
                else
                {
                    outputMessage = $"[CQ:at,qq={user_id}]，服务器内没有其他牛子！快邀请一只牛子吧！";
                }
            }
            else
            {
                outputMessage =
                    $"[CQ:at,qq={user_id}]，你的牛子“{challengerDick.NickName}”，体力值不足，无法进行跨服斗牛！当前体力值为{currentEnergy}/240";
            }
        }
        else
        {
            outputMessage = TipsMessage.DickNotFound(user_id);
        }

        await WebSocketClient.Send(SendGroupMessage.Generate(outputMessage, group_id));
    }
}