using System.Data.SQLite;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CoreLibrary;
using CoreLibrary.DataBase;
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
                        case "状态":
                        {
                            Console.WriteLine("收到消息：" + groupMessage.raw_message);
                            var messageObject = new
                            {
                                action = "send_group_msg_rate_limited",
                                @params = new
                                {
                                    groupMessage.group_id,
                                    message = "牛子系统处于施工当中！"
                                }
                            };
                            var message = JsonSerializer.Serialize(messageObject);
                            await SendMessage(message);
                            break;
                        }
                        case "牛子系统":
                        {
                            Console.WriteLine("主菜单");
                            var messageObject = new
                            {
                                action = "send_group_msg_rate_limited",
                                @params = new
                                {
                                    groupMessage.group_id,
                                    message = "牛子系统正在升级，敬请期待！第一时间了解详情请加QQ群：745297798！目前功能：1.生成牛子 2.我的牛子 3.锻炼牛子"
                                }
                            };
                            var message = JsonSerializer.Serialize(messageObject);
                            await SendMessage(message);
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

                            var messageObject = new
                            {
                                action = "send_group_msg_rate_limited",
                                @params = new
                                {
                                    groupMessage.group_id,
                                    message =
                                        stringMessage
                                }
                            };
                            var message = JsonSerializer.Serialize(messageObject);
                            await SendMessage(message);
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
                                    $"用户{groupMessage.user_id}，你的牛子“{newDick.NickName}”，牛子身份证[{newDick.GUID}]，目前长度为{newDick.Length:F2}cm，当前体力状况：[{newDick.Energy}/240]";
                            }
                            else
                            {
                                stringMessage = $"用户{groupMessage.user_id}，你还没有牛子！请使用“生成牛子”指令，生成一只牛子。";
                            }


                            var messageObject = new
                            {
                                action = "send_group_msg_rate_limited",
                                @params = new
                                {
                                    groupMessage.group_id,
                                    message =
                                        stringMessage
                                }
                            };
                            var message = JsonSerializer.Serialize(messageObject);
                            await SendMessage(message);
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
                                var currentEnergy = await DickFighterDataBase.CheckEnergy(newDick.GUID);
                                if (currentEnergy > 40)
                                {
                                    //体力值足够
                                    var newEnergy = currentEnergy - 40;
                                    await DickFighterDataBase.UpdateDickEnergy(guid: newDick.GUID, energy: newEnergy);
                                    var lengthDifference = GenerateRandom.GetRandomDouble(-10, 20);
                                    newDick.Length += lengthDifference;
                                    await DickFighterDataBase.UpdateDickLength(newDick.Length, newDick.GUID);
                                    stringMessage =
                                        $"用户{groupMessage.user_id}，你的牛子“{newDick.NickName}”，牛子身份证[{newDick.GUID}]，锻炼成功！消耗40体力值，当前体力值为{newEnergy}/240，锻炼使得牛子长度变化{lengthDifference:F3}cm，目前牛子长度为{newDick.Length:F2}cm";
                                }
                                else
                                {
                                    stringMessage =
                                        $"用户{groupMessage.user_id}，你的牛子“{newDick.NickName}”，牛子身份证[{newDick.GUID}]，体力值不足，无法锻炼！当前体力值为{currentEnergy}/240";
                                }
                            }
                            else
                            {
                                stringMessage = $"[CQ:at,qq={groupMessage.user_id}]，你还没有牛子！请使用“生成牛子”指令，生成一只牛子。";
                            }
                            var messageObject = new
                            {
                                action = "send_group_msg_rate_limited",
                                @params = new
                                {
                                    groupMessage.group_id,
                                    message =
                                        stringMessage
                                }
                            };
                            var message = JsonSerializer.Serialize(messageObject);
                            await SendMessage(message);

                            break;
                        }
                        default:
                        {
                            if (groupMessage.raw_message != null && groupMessage.raw_message.Contains("改牛子名"))
                            {
                                string stringMessage;
                                Console.WriteLine("尝试修改牛子名字！");
                                Console.WriteLine("groupMessage.raw_message: " + groupMessage.raw_message);
                                var (newName, ifNeedEdit) = 正则表达式.改牛子名(groupMessage.raw_message);
                                var (item1, newDick) =
                                    await DickFighterDataBase.CheckPersonalDick(groupMessage.user_id,
                                        groupMessage.group_id);
                                if (ifNeedEdit)
                                {
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

                                    var messageObject = new
                                    {
                                        action = "send_group_msg_rate_limited",
                                        @params = new
                                        {
                                            groupMessage.group_id,
                                            message =
                                                stringMessage
                                        }
                                    };
                                    var message = JsonSerializer.Serialize(messageObject);
                                    await SendMessage(message);
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