using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CoreLibrary;
using CoreLibrary.DataBase;
using CoreLibrary.SendMessages;
using CoreLibrary.Tools;

namespace DickFighterBot;

public class WebSocketClient
{
    private static ClientWebSocket clientWebSocket;
    private static string databaseFolderPath;

    public static async Task Main()
    {
        // 组合数据库文件路径


        await DickFighterDataBase.InitializeDataBase();

        clientWebSocket = new ClientWebSocket();
        var serverUri = new Uri("ws://192.168.2.168:3001");

        try
        {
            await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("WebSocket服务器连接成功！");

            // 启动消息接收任务
            var receiveTask = ReceiveMessages();


            // 等待消息接收任务完成
            await receiveTask;

            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed.",
                CancellationToken.None);
            Console.WriteLine("Disconnected from WebSocket server.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("WebSocket服务器连接失败，错误信息：" + ex.Message);
        }
        finally
        {
            clientWebSocket.Dispose();
        }
    }

    private static async Task SendMessage(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
            CancellationToken.None);
        Console.WriteLine("已发送消息 " + message);
    }

    private static async Task ReceiveMessages()
    {
        var buffer = new byte[1024];
        while (clientWebSocket.State == WebSocketState.Open)
        {
            var result =
                await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("收到JSON: " + receivedMessage);

                // 解析接收到的 JSON 消息
                try
                {
                    var groupMessage = JsonSerializer.Deserialize<Message.GroupMessage>(receivedMessage);

                    switch (groupMessage?.raw_message)
                    {
                        case "牛子系统":
                        {
                            var stringMessage =
                                "牛子系统正在升级，敬请期待！第一时间了解详情请加QQ群：745297798！目前功能：1.生成牛子 2.我的牛子 3.锻炼牛子 4.改牛子名";
                            await SendMessage(GroupMessage.SendGroupMessage(stringMessage, groupMessage.group_id));
                            break;
                        }
                        case "生成牛子":
                        {
                            //判断是否已经有了牛子
                            var checkResult =
                                await DickFighterDataBase.CheckPersonalDick(groupMessage.user_id,
                                    groupMessage.group_id);
                            var ifExist = checkResult.Item1;
                            string stringMessage;
                            if (ifExist)
                            {
                                stringMessage = $"用户{groupMessage.user_id}，你已经有了一只牛子，请不要贪心！";
                            }
                            else
                            {
                                Console.WriteLine("尝试生成一只牛子！");
                                var newGuid = Guid.NewGuid().ToString();
                                var newDick = new Dick(groupMessage.user_id, "不知名的牛子", 0,
                                    GenerateRandom.GetRandomDouble(5d, 15d), newGuid);

                                await DickFighterDataBase.GenerateNewDick(groupMessage.user_id, groupMessage.group_id,
                                    newDick);

                                stringMessage =
                                    $"用户{groupMessage.user_id}，你的牛子[{newDick.GUID}]已经成功生成，初始长度为{newDick.Length:F3}cm";
                            }

                            if (groupMessage.group_id == 836369648)
                            {
                                stringMessage = "牢Rin还没睡觉，你这是想牛子被封号";
                            }

                            await SendMessage(GroupMessage.SendGroupMessage(stringMessage, groupMessage.group_id));
                            break;
                        }
                        case "我的牛子":
                        {
                            string stringMessage;

                            //查询是否已经存在牛子
                            var (item1, newDick) =
                                await DickFighterDataBase.CheckPersonalDick(groupMessage.user_id,
                                    groupMessage.group_id);
                            if (item1)
                            {
                                newDick.Energy = await DickFighterDataBase.CheckEnergy(newDick.GUID);
                                stringMessage =
                                    $"用户{groupMessage.user_id}，你的牛子“{newDick.NickName}”，目前长度为{newDick.Length:F2}cm，当前体力状况：[{newDick.Energy}/240]";
                            }
                            else
                            {
                                stringMessage = $"用户{groupMessage.user_id}，你还没有牛子！请使用“生成牛子”指令，生成一只牛子。";
                            }

                            if (groupMessage.group_id == 836369648)
                            {
                                stringMessage = "牢Rin还没睡觉，你这是想牛子被封号";
                            }

                            await SendMessage(GroupMessage.SendGroupMessage(stringMessage, groupMessage.group_id));
                            break;
                        }
                        case "锻炼牛子":
                        {
                            string stringMessage;

                            //查询是否已经存在牛子
                            var (item1, newDick) =
                                await DickFighterDataBase.CheckPersonalDick(groupMessage.user_id,
                                    groupMessage.group_id);
                            if (item1)
                            {
                                //检查体力值
                                newDick.Energy = await DickFighterDataBase.CheckEnergy(newDick.GUID);
                                var currentEnergy = newDick.Energy;
                                if (currentEnergy >= 40)
                                {
                                    //体力值足够
                                    var newEnergy = currentEnergy - 40;
                                    await DickFighterDataBase.UpdateDickEnergy(guid: newDick.GUID, energy: newEnergy);
                                    var lengthDifference = GenerateRandom.GetRandomDouble(-10, 20);
                                    newDick.Length += lengthDifference;
                                    await DickFighterDataBase.UpdateDickLength(newDick.Length, newDick.GUID);
                                    stringMessage =
                                        $"用户{groupMessage.user_id}，你的牛子“{newDick.NickName}”，锻炼成功！消耗40体力值，当前体力值为{newEnergy}/240，锻炼使得牛子长度变化{lengthDifference:F3}cm，目前牛子长度为{newDick.Length:F2}cm";
                                }
                                else
                                {
                                    stringMessage =
                                        $"用户{groupMessage.user_id}，你的牛子“{newDick.NickName}”，体力值不足，无法锻炼！当前体力值为{currentEnergy}/240";
                                }
                            }
                            else
                            {
                                stringMessage = $"[CQ:at,qq={groupMessage.user_id}]，你还没有牛子！请使用“生成牛子”指令，生成一只牛子。";
                            }

                            if (groupMessage.group_id == 836369648)
                            {
                                stringMessage = "牢Rin还没睡觉，你这是想牛子被封号";
                            }

                            await SendMessage(GroupMessage.SendGroupMessage(stringMessage, groupMessage.group_id));

                            break;
                        }
                        case "斗牛":
                        {
                            string stringMessage;
                            var (item1, challengerDick) =
                                await DickFighterDataBase.CheckPersonalDick(groupMessage.user_id,
                                    groupMessage.group_id);
                            if (item1)
                            {
                                challengerDick.Energy = await DickFighterDataBase.CheckEnergy(challengerDick.GUID);
                                var currentEnergy = challengerDick.Energy;
                                if (currentEnergy >= 20)
                                {
                                    //体力充足，扣取体力以后决斗
                                    challengerDick.Energy -= 20;
                                    await DickFighterDataBase.UpdateDickEnergy(challengerDick.Energy,
                                        challengerDick.GUID);

                                    //Todo: 查询群内其他人的牛子，随机选择一只牛子进行对战
                                    var defenderDick = await DickFighterDataBase.GetRandomDick(groupMessage.group_id,
                                        challengerDick.GUID); //防止自己打自己
                                    if (defenderDick != null)
                                    {
                                        var battleResult = FightCalculation.Calculate(challengerDick.Length,
                                            defenderDick.Length, 0, challengerDick.Length - defenderDick.Length);
                                        var stringMessage1 =
                                            $"用户{groupMessage.user_id}，你的牛子“{challengerDick.NickName}”，消耗20点体力，向 [CQ:at,qq={defenderDick.Belongings}] 发起了斗牛！根据牛科院物理研究所计算，你的牛子胜率为{battleResult.winRatePct:F1}%。";
                                        string stringMessage2;

                                        //更新牛子长度
                                        challengerDick.Length += battleResult.challengerChange;
                                        defenderDick.Length += battleResult.defenderChange;
                                        await DickFighterDataBase.UpdateDickLength(challengerDick.Length,
                                            challengerDick.GUID);
                                        await DickFighterDataBase.UpdateDickLength(defenderDick.Length,
                                            defenderDick.GUID);

                                        if (battleResult.isWin)
                                        {
                                            stringMessage2 =
                                                $"你的牛子“{challengerDick.NickName}”在斗牛当中获得了胜利！长度变化为{battleResult.challengerChange:F3}cm，目前长度为{challengerDick.Length:F2}cm。对方牛子“{defenderDick.NickName}”长度变化为{battleResult.defenderChange:F3}cm，目前长度为{defenderDick.Length:F2}cm。";
                                        }
                                        else
                                        {
                                            stringMessage2 =
                                                $"你的牛子“{challengerDick.NickName}”在斗牛当中遗憾地失败！长度变化为{battleResult.challengerChange:F3}cm，目前长度为{challengerDick.Length:F2}cm。对方牛子“{defenderDick.NickName}”长度变化为{battleResult.defenderChange:F3}cm，目前长度为{defenderDick.Length:F2}cm";
                                        }

                                        stringMessage = stringMessage1 + stringMessage2;
                                    }
                                    else
                                    {
                                        stringMessage = $"[CQ:at,qq={groupMessage.user_id}]，群内没有其他牛子！快邀请一只牛子进群吧！";
                                    }
                                }
                                else
                                {
                                    stringMessage =
                                        $"用户{groupMessage.user_id}，你的牛子“{challengerDick.NickName}”，体力值不足，无法斗牛！当前体力值为{currentEnergy}/240";
                                }
                            }
                            else
                            {
                                stringMessage = $"[CQ:at,qq={groupMessage.user_id}]，你还没有牛子！请使用“生成牛子”指令，生成一只牛子。";
                            }

                            if (groupMessage.group_id == 836369648)
                            {
                                stringMessage = "牢Rin还没睡觉，你这是想牛子被封号";
                            }

                            await SendMessage(GroupMessage.SendGroupMessage(stringMessage, groupMessage.group_id));

                            //Todo: 发送消息
                            break;
                        }
                        default:
                        {
                            if (groupMessage.raw_message != null && groupMessage.raw_message.Contains("改牛子名"))
                            {
                                Console.WriteLine("尝试修改牛子名字！");
                                Console.WriteLine("groupMessage.raw_message: " + groupMessage.raw_message);
                                var (newName, ifNeedEdit) = 正则表达式.改牛子名(groupMessage.raw_message);
                                var (item1, newDick) =
                                    await DickFighterDataBase.CheckPersonalDick(groupMessage.user_id,
                                        groupMessage.group_id);
                                if (ifNeedEdit)
                                {
                                    string stringMessage;
                                    if (item1)
                                    {
                                        //如果需要修改名字并且有牛子
                                        stringMessage = $"[CQ:at,qq={groupMessage.user_id}]，你的牛子名字已经修改为[{newName}]！";
                                        await DickFighterDataBase.UpdateDickNickName(groupMessage.user_id,
                                            groupMessage.group_id,
                                            newName);
                                    }
                                    else
                                    {
                                        stringMessage = $"[CQ:at,qq={groupMessage.user_id}]，你还没有牛子！请使用“生成牛子”指令，生成一只牛子。";
                                    }

                                    await SendMessage(GroupMessage.SendGroupMessage(stringMessage,
                                        groupMessage.group_id));
                                }
                            }

                            break;
                        }
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine("Error parsing JSON message: " + ex.Message);
                }
            }
        }
    }
}