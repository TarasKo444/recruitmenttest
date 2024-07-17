using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RecruitmentTest.Application.Services;

namespace RecruitmentTest.Api.Controllers
{
    public class WebSocketApiController : ControllerBase
    {
        private readonly AuthService _authService;
        private const string ExternalWebSocketUri = "ws://platform.fintacharts.com/api/streaming/ws/v1/realtime"; // Replace with the actual external WebSocket URI

        public WebSocketApiController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("api/ws")]
        public async Task Get(
            [FromQuery] string instrumentId,
            [FromQuery] string provider,
            [FromQuery] List<string> kinds)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var credentials = await _authService.GetCredentialsAsync();
                
                using var clientWebSocket = new ClientWebSocket();
                await clientWebSocket.ConnectAsync(new Uri(ExternalWebSocketUri + $"?token={credentials.AccessToken}"),
                    CancellationToken.None);

                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await ProxyWebSockets(webSocket, clientWebSocket, new()
                {
                    Type = "l1-subscription",
                    Id = "1",
                    InstrumentId = instrumentId,
                    Provider = provider,
                    Subscribe = true,
                    Kinds = kinds
                });
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private static async Task ProxyWebSockets(WebSocket clientWebSocket, ClientWebSocket serverWebSocket, SendDto sendDto)
        {
            var buffer = new byte[1024 * 4];
            var clientReceiveTask = clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var serverReceiveTask = serverWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            var jsonString = JsonSerializer.Serialize(sendDto);
            var jsonBuffer = Encoding.UTF8.GetBytes(jsonString);
            var sendTask = serverWebSocket.SendAsync(new ArraySegment<byte>(jsonBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

            while (clientWebSocket.State == WebSocketState.Open && serverWebSocket.State == WebSocketState.Open)
            {
                var completedTask = await Task.WhenAny(clientReceiveTask, serverReceiveTask);
                await sendTask;
                
                if (completedTask == clientReceiveTask)
                {
                    var clientReceiveResult = await clientReceiveTask;

                    if (clientReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        await serverWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Client closed connection", CancellationToken.None);
                    }
                    else
                    {
                        await serverWebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, clientReceiveResult.Count), clientReceiveResult.MessageType, clientReceiveResult.EndOfMessage, CancellationToken.None);
                        clientReceiveTask = clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    }
                }
                else
                {
                    var serverReceiveResult = await serverReceiveTask;

                    if (serverReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Server closed connection", CancellationToken.None);
                    }
                    else
                    {
                        await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, serverReceiveResult.Count), serverReceiveResult.MessageType, serverReceiveResult.EndOfMessage, CancellationToken.None);
                        serverReceiveTask = serverWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    }
                }
            }
        }
    }
}

public class SendDto
{
    public required string Type { get; set; }
    public required string Id { get; set; }
    public required string InstrumentId { get; set; }
    public required string Provider { get; set; }
    public required bool Subscribe { get; set; }
    public required List<string> Kinds { get; set; }
}