using System.Net.WebSockets;
using System.Text;

namespace BattleShipSocket.Typings;

public class Client
{
    private Guid _clientId;
    private WebSocket _clientSocket;
    
    public override string ToString()
    {
        return _clientId.ToString();
    }
    public Guid ClientId
    {
        get => _clientId;
    }
    public WebSocket ClientSocket
    {
        get => _clientSocket;
    }
    public Guid RoomId { get; set; }
    public Client(Guid clientId, WebSocket clientSocket)
    {
        _clientId = clientId;
        _clientSocket = clientSocket;
    }

    public void Dispose()
    {
        _clientSocket.Dispose();
    }
    
    public async void Emit(string eventName, string data)
    {
        string message = Newtonsoft.Json.JsonConvert.SerializeObject(new WebsocketMessage(eventName, data));
        
        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        await _clientSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}