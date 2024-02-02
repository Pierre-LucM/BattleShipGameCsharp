using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using BattleShipEventManager;
using BattleShipSocket.Enum;
using BattleShipSocket.Interfaces;
using BattleShipSocket.Typings;

namespace BattleShipSocket;

public class Websocket : IDisposable
{
    private const int BufferSize = 1024;
    private HttpListener _httpListener;
    private string _url;
    private ushort _port;
    private EventManager _eventManager = EventManager.Instance;
    private Dictionary<string, Client> _clients = new Dictionary<string, Client>();
    public Dictionary<string, Client> Clients => _clients;

    public Websocket(string url, ushort port)
    {
        _url = url;
        _port = port;
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(url + ":" + port + "/");
    }

    public async Task Start()
    {
        _httpListener.Start();
        Console.WriteLine($"Server listening on {_url}:{_port}/...");

        while (true)
        {
            var context = await _httpListener.GetContextAsync();

            if (context.Request.IsWebSocketRequest)
            {
                ProcessWebSocketRequest(context);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private async void ProcessWebSocketRequest(HttpListenerContext context)
    {
        var webSocket = await context.AcceptWebSocketAsync(null);
        // convert HtpListenerWebSocketContext to WebSocket
        var socket = webSocket.WebSocket;
        Console.WriteLine("WebSocket connection established.");

        Guid clientId = Guid.NewGuid();
        this._clients.Add(clientId.ToString(), new Client(clientId, socket));

        this.Emit(EnumEvents.ClientConnected, "Client Connected Successfully " + clientId, clientId.ToString());
        // Handle WebSocket messages
        await HandleWebSocketMessages(clientId.ToString(), socket);
    }

    private async Task HandleWebSocketMessages(string clientId, WebSocket webSocket)
    {
        var buffer = new byte[BufferSize];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                WebsocketMessage? message =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<WebsocketMessage>(receivedMessage);
                // if EventName is not in EnumEvents it automatically throws an exception
                _eventManager.Emit(message!.EventName, message.Data, clientId);

                // Echo the message back to the client
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text,
                    result.EndOfMessage, CancellationToken.None);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                _eventManager.Emit(EnumHelper.GetStringAttributeValueFromEnum(EnumEvents.ClientDisconnected), "",
                    clientId);
                Console.WriteLine("Received a close message from client.");
                Console.WriteLine($"Client {clientId} disconnected");
                this._clients.Remove(clientId);
              
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
        }
    }

    private async Task SendWebSocketMessage(WebSocket socket, string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true,
            CancellationToken.None);
    }


    public void On(EnumEvents eventName, Action<string, string> callback)
    {
        _eventManager.On(EnumHelper.GetStringAttributeValueFromEnum(eventName), callback);
    }

    public async void Emit(EnumEvents eventName, string data, string clientId)
    {
        var message =
            Newtonsoft.Json.JsonConvert.SerializeObject(
                new WebsocketMessage(EnumHelper.GetStringAttributeValueFromEnum(eventName), data));
        Console.WriteLine(message);
        bool isClient = this._clients.TryGetValue(clientId, out Client? client);
        if (!isClient)
        {
            throw new Exception("Client does not exist or is not connected");
        }

        await this.SendWebSocketMessage(client!.ClientSocket!, message);
        _eventManager.Emit(EnumHelper.GetStringAttributeValueFromEnum(eventName), data, clientId);
    }

    public async void EmitToRoom(EnumEvents eventName, string data, Room room)
    {
        var message =
            Newtonsoft.Json.JsonConvert.SerializeObject(
                new WebsocketMessage(EnumHelper.GetStringAttributeValueFromEnum(eventName), data));
        Console.WriteLine(message);
        foreach (var client in room.Clients)
        {
            await this.SendWebSocketMessage(client.ClientSocket!, message);
        }
    }

    public void Dispose()
    {
        _httpListener.Stop();
        _httpListener.Close();
    }
}