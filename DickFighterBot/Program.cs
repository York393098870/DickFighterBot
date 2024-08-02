using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CoreLibrary.config;
using CoreLibrary.DataBase;
using NLog;

namespace DickFighterBot;

public class WebSocketClient
{
    private static ClientWebSocket clientWebSocket;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); //获取日志记录器

    public static async Task Main()
    {
        await DickFighterDataBase.Initialize(); //初始化数据库

        clientWebSocket = new ClientWebSocket();

        //加载配置文件
        var configFile = ConfigLoader.Load();
        var serverUri = new Uri($"ws://{configFile.MainSettings.ws_host}:{configFile.MainSettings.port}");

        try
        {
            await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
            Logger.Info("WebSocket服务器连接成功！");

            // 启动消息接收任务
            var receiveTask = ReceiveFromServer();

            // 等待消息接收任务完成
            await receiveTask;

            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "连接已关闭。",
                CancellationToken.None);
            Logger.Info("从WebSocket服务器断开连接！");
        }
        catch (Exception ex)
        {
            Logger.Fatal("主程序出现致命错误：" + ex.Message);
            Logger.Fatal("错误详情：" + ex.StackTrace);
            Logger.Info("按任意键退出程序。");
            Console.ReadKey();
        }
        finally
        {
            clientWebSocket.Dispose();
        }
    }

    public static async Task Send(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
            CancellationToken.None);

        await Task.Delay(ConfigLoader.Load().MainSettings.Interval); //延迟一定的时间再发送下一条消息
    }

    private static async Task ReceiveFromServer()
    {
        var buffer = new byte[2048];
        while (clientWebSocket.State == WebSocketState.Open)
        {
            var result =
                await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            Logger.Trace($"收到类型为[{result.MessageType}]的消息。");
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Logger.Trace("收到消息：" + receivedMessage);

            var dispatcher = new CommandDispatcher();

            try
            {
                var messageReceived = JsonSerializer.Deserialize<Message.GroupMessage>(receivedMessage); //反序列化收到的消息

                if (messageReceived is { user_id: > 0, group_id: > 0 })
                {
                    await dispatcher.Dispatch(messageReceived.user_id, messageReceived.group_id, messageReceived);
                }
            }
            catch (JsonException ex)
            {
                Logger.Warn($"解析JSON时出现异常：{ex.Message} ");
            }
        }
    }
}