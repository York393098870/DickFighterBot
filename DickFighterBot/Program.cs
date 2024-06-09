using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using DickFighterBot;

public class WebSocketClientExample
{
    private static ClientWebSocket clientWebSocket;

    public static async Task Main()
    {
        clientWebSocket = new ClientWebSocket();
        var serverUri = new Uri("ws://192.168.2.168:3001"); // 替换为实际的 WebSocket 服务器地址

        try
        {
            await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("WebSocket服务器连接成功！");

            // 启动消息接收任务
            var receiveTask = ReceiveMessages();

            // 在此处可以发送 WebSocket 消息

            var messageObject = new
            {
                action = "send_group_msg_rate_limited",
                @params = new
                {
                    group_id = 745297798,
                    message = "Bot连接成功！"
                }
            };
            var message = JsonSerializer.Serialize(messageObject);
            await SendMessage(message);

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

                    if (groupMessage == null)
                    {
                        Console.WriteLine("出现问题，收到了非正确的信息！");
                        continue;
                    }

                    if (groupMessage.message != null && groupMessage.message.Contains('居') &&
                        groupMessage.group_id == 599368159)
                    {
                        Console.WriteLine("有人提到了居！");
                        var messageObject = new
                        {
                            action = "send_group_msg_rate_limited",
                            @params = new
                            {
                                group_id = groupMessage.group_id,
                                message = "居不是什么，居就是居！"
                            }
                        };
                        var message = JsonSerializer.Serialize(messageObject);
                        await SendMessage(message);
                    }

                    if (groupMessage.raw_message != null && groupMessage.raw_message.Contains("原神") &&
                        groupMessage.group_id == 745297798)
                    {
                        Console.WriteLine("准备发送消息：原神启动！");
                        var messageObject = new
                        {
                            action = "send_group_msg_rate_limited",
                            @params = new
                            {
                                group_id = groupMessage.group_id,
                                message = "启动！"
                            }
                        };
                        var message = JsonSerializer.Serialize(messageObject);
                        await SendMessage(message);
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