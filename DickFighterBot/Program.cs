using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Xml;
using CoreLibrary;
using CoreLibrary.DataBase;
using CoreLibrary.SendMessages;
using CoreLibrary.Tools;
using DickFighterBot.Functions;

namespace DickFighterBot;

public class WebSocketClient
{
    private static ClientWebSocket clientWebSocket;
    private static string databaseFolderPath;

    public static async Task Main()
    {
        await DickFighterDataBase.InitializeDataBase(); //初始化数据库

        clientWebSocket = new ClientWebSocket();

        //读取XML配置文件，如果已经存在config.xml文件，则直接读取，否则读取运行目录下的config.xml文件
        var xmlDocument = new XmlDocument();
        var configPath = Path.Combine(ProgramPath.MathPath, "config.xml");
        if (File.Exists(configPath))
        {
            xmlDocument.Load(configPath);
            Console.WriteLine("检测到本地配置文件，已加载！");
        }
        else
        {
            xmlDocument.Load("config.xml");
            Console.WriteLine("没有检测到本地配置文件，已加载运行目录下默认配置文件！");
        }

        var addressNode = xmlDocument.SelectSingleNode("/config/server/address");
        var address = addressNode.InnerText;
        var fullAddress = $"ws://{address}:3001";
        var serverUri = new Uri(fullAddress);

        try
        {
            await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("WebSocket服务器连接成功！");

            // 启动消息接收任务
            var receiveTask = ReceiveMessages();

            // 等待消息接收任务完成
            await receiveTask;

            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "连接已关闭。",
                CancellationToken.None);
            Console.WriteLine("从WebSocket服务器断开连接！");
        }
        catch (Exception ex)
        {
            Console.WriteLine("WebSocket服务器连接失败，错误信息：" + ex.Message);
            Console.ReadKey();
        }
        finally
        {
            clientWebSocket.Dispose();
        }
    }

    public static async Task SendMessage(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
            CancellationToken.None);
    }

    private static async Task ReceiveMessages()
    {
        var buffer = new byte[1024];
        while (clientWebSocket.State == WebSocketState.Open)
        {
            var result =
                await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType != WebSocketMessageType.Text) continue;

            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

            try
            {
                var groupMessage = JsonSerializer.Deserialize<Message.GroupMessage>(receivedMessage); //反序列化收到的消息

                switch (groupMessage?.raw_message)
                {
                    case "状态":
                    {
                        await CurrentStatus.Main(groupMessage.group_id);
                        break;
                    }
                    case "牛子系统":
                    {
                        await ShowFunctions.ShowHelp(groupMessage.group_id);
                        break;
                    }
                    case "生成牛子":
                    {
                        await GenerateNewDick.Main(user_id: groupMessage.user_id, group_id: groupMessage.group_id);
                        break;
                    }
                    case "我的牛子":
                    {
                        await CheckMyDick.Main(user_id: groupMessage.user_id, group_id: groupMessage.group_id);
                        break;
                    }
                    case "锻炼牛子":
                    {
                        await Exercise.Main(user_id: groupMessage.user_id, group_id: groupMessage.group_id);
                        break;
                    }
                    case "润滑度":
                    {
                        await 润滑度.Main(user_id: groupMessage.user_id, group_id: groupMessage.group_id);
                        break;
                    }
                    case "斗牛":
                    {
                        await 斗牛.Main(user_id: groupMessage.user_id, group_id: groupMessage.group_id);
                        break;
                    }
                    default:
                    {
                        if (groupMessage.raw_message != null && groupMessage.raw_message.Contains("改牛子名"))
                        {
                            await ChangeDickName.Main(user_id: groupMessage.user_id, group_id: groupMessage.group_id,
                                rawMessage: groupMessage.raw_message);
                        }
                        else if (groupMessage.raw_message != null && groupMessage.raw_message == "waifu" &&
                                 groupMessage.user_id == 393098870)
                        {
                            //Todo:准备做waifu的功能
                        }

                        break;
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine("解析JSON时出现异常： " + ex.Message);
            }
        }
    }
}