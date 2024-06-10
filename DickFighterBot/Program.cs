using System.Data.SQLite;
using System.Net.WebSockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using CoreLibrary;
using CoreLibrary.DataBase;
using CoreLibrary.Tools;

namespace DickFighterBot;

public class WebSocketClient
{
    private static ClientWebSocket clientWebSocket;

    public static async Task Main()
    {
        var dataBaseSource = Path.Combine(Environment.CurrentDirectory, "dickfightdatabase.db");
        Console.WriteLine(Environment.CurrentDirectory);
        var connectionString = $"Data Source={dataBaseSource};Version=3;";

        await using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            await using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = """
                                      
                                                          CREATE TABLE IF NOT EXISTS BasicInformation (GUID TEXT PRIMARY KEY,
                                                              DickBelongings INTEGER,
                                                              NickName TEXT,
                                                              Length REAL,
                                                              Gender INTEGER,
                                                              GroupNumber INTEGER
                                                          );CREATE TABLE IF NOT EXISTS BattleRecord (
                                                              BattleID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                              ChallengerGUID TEXT,
                                                              DefenderGUID TEXT,
                                                              IsWin INTEGER,
                                                              Time INTEGER
                                                          );CREATE TABLE IF NOT EXISTS ExerciseRecord (
                                                              ExerciseID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                              DickGUID TEXT,
                                                              ExerciseTime INTEGER,
                                                              ExerciseChange REAL,
                                                              NextExerciseTime INTEGER
                                                          );
                                      """;
                command.ExecuteNonQuery();
            }
        }

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
            Console.WriteLine("WebSocket服务器连接失败，错误信息： " + ex.Message);
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
        const string connectionString = "Data Source=dickfightdatabase.db;Version=3;";
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

                    if (groupMessage?.raw_message == "状态")
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
                    }
                    else if (groupMessage?.raw_message == "牛子系统")
                    {
                        Console.WriteLine("主菜单");
                        var messageObject = new
                        {
                            action = "send_group_msg_rate_limited",
                            @params = new
                            {
                                groupMessage.group_id,
                                message = "牛子系统正在升级，敬请期待！第一时间了解详情请加QQ群：745297798！目前功能：1.生成牛子 2.我的牛子 3.锻炼（开发中）"
                            }
                        };
                        var message = JsonSerializer.Serialize(messageObject);
                        await SendMessage(message);
                    }
                    else if (groupMessage?.raw_message == "生成牛子")
                    {
                        //判断是否已经有了牛子
                        bool ifExist;
                        await using (var connection = new SQLiteConnection(connectionString))
                        {
                            connection.Open();

                            var dickBelongings = groupMessage.user_id;
                            var groupNumber = groupMessage.group_id;
                            //查询是否已经存在牛子
                            await using (var command = new SQLiteCommand(connection))
                            {
                                command.CommandText =
                                    "SELECT GUID FROM BasicInformation WHERE DickBelongings = @DickBelongings AND GroupNumber = @GroupNumber";
                                command.Parameters.AddWithValue("@DickBelongings", dickBelongings);
                                command.Parameters.AddWithValue("@GroupNumber", groupNumber);

                                await using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        //已经存在牛子
                                        ifExist = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("不存在对应的行，返回值为FALSE");
                                        ifExist = false;
                                    }
                                }
                            }
                        }

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

                            await using (var connection = new SQLiteConnection(connectionString))
                            {
                                connection.Open();

                                await using (var command = new SQLiteCommand(connection))
                                {
                                    command.CommandText =
                                        "INSERT INTO BasicInformation (GUID, DickBelongings, NickName, Length, Gender, GroupNumber) " +
                                        "VALUES (@GUID, @DickBelongings, @NickName, @Length, @Gender, @GroupNumber)";
                                    command.Parameters.AddWithValue("@GUID", newDick.GUID);
                                    command.Parameters.AddWithValue("@DickBelongings", groupMessage.user_id);
                                    command.Parameters.AddWithValue("@NickName", "不知名的牛子");
                                    command.Parameters.AddWithValue("@Length", newDick.Length);
                                    command.Parameters.AddWithValue("@Gender", 1);
                                    command.Parameters.AddWithValue("@GroupNumber", groupMessage.group_id);
                                    command.ExecuteNonQuery();
                                }
                            }

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
                    }
                    else if (groupMessage?.raw_message == "我的牛子")
                    {
                        string stringMessage;
                        await using (var connection = new SQLiteConnection(connectionString))
                        {
                            connection.Open();

                            var dickBelongings = groupMessage.user_id;
                            var groupNumber = groupMessage.group_id;
                            //查询是否已经存在牛子

                            var (item1, newDick) =
                                await DickFighterDataBase.CheckPersonalDick(dickBelongings, groupNumber);
                            if (item1)
                            {
                                stringMessage =
                                    $"用户{groupMessage.user_id}，你的牛子[{newDick.NickName}]，牛子身份证{newDick.GUID}，目前长度为{newDick.Length:F2}cm";
                            }
                            else
                            {
                                stringMessage = $"用户{groupMessage.user_id}，你还没有牛子！请使用“生成牛子”指令，生成一只牛子。";
                            }
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
                    else if (groupMessage?.raw_message == "锻炼牛子")
                    {
                        string stringMessage;


                        var dickBelongings = groupMessage.user_id;
                        var groupNumber = groupMessage.group_id;
                        //查询是否已经存在牛子
                        var (item1, newDick) =
                            await DickFighterDataBase.CheckPersonalDick(dickBelongings, groupNumber);
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