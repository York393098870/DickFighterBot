using System.Data.SQLite;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CoreLibrary;
using CoreLibrary.Tools;
using DickFighterBot;

public class WebSocketClientExample
{
    private static ClientWebSocket clientWebSocket;

    public static async Task Main()
    {
        const bool deBugMode = false;
        if (deBugMode)
        {
            //删除原有的数据库
            File.Delete("dickfightdatabase.db");
        }

        //首先检测当前目录下是否存在已有的数据库，否则创建SQLite数据库
        if (!File.Exists("dickfightdatabase.db"))
        {
            // 创建新的 SQLite 数据库
            SQLiteConnection.CreateFile("dickfightdatabase.db");
        }

        const string connectionString = "Data Source=dickfightdatabase.db;Version=3;";
        await using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            await using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = @"
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
                    );";
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
                                    group_id = groupMessage.group_id,
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
                                    group_id = groupMessage.group_id,
                                    message = "牛子系统正在升级，敬请期待！第一时间了解详情请加QQ群：745297798！目前功能：1.生成牛子 2.我的牛子 3.锻炼"
                                }
                            };
                            var message = JsonSerializer.Serialize(messageObject);
                            await SendMessage(message);
                            break;
                        }
                        case "生成牛子":
                        {
                            //判断是否已经有了牛子
                            bool ifExist;
                            string existGuid;
                            await using (var connection = new SQLiteConnection(connectionString))
                            {
                                connection.Open();

                                var dickBelongings = groupMessage.user_id; // 你的具体数值
                                var groupNumber = groupMessage.group_id; // 你的具体数值

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
                                            existGuid = reader["GUID"].ToString();
                                        }
                                        else
                                        {
                                            Console.WriteLine("不存在对应的行，返回值为FALSE");
                                        }
                                    }
                                }
                            }

                            Console.WriteLine("尝试生成一只牛子！");
                            var newGuid = Guid.NewGuid().ToString();
                            var newDick = new Dick(belongings: groupMessage.user_id, nickName: "不知名的牛子", gender: 0,
                                GenerateRandom.GetRandomDouble(5d, 15d), guid: newGuid);

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


                            var messageObject = new
                            {
                                action = "send_group_msg_rate_limited",
                                @params = new
                                {
                                    group_id = groupMessage.group_id,
                                    message =
                                        $"用户{groupMessage.user_id}，你的牛子{newDick.GUID}已经成功生成，初始长度为{newDick.Length:F3}cm。"
                                }
                            };
                            var message = JsonSerializer.Serialize(messageObject);
                            await SendMessage(message);
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