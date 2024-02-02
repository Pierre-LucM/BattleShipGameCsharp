using BattleShipEventManager;
using BattleShipSocket;
using BattleShipSocket.Enum;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var httpListenerPrefix = "http://localhost";
        var server = WebsocketManager.WebsocketManagerInstance;
        server.OnConnection((clientId) =>
        {
            Console.WriteLine($"Client {clientId} connected");
        });
        server.OnRequestGameCreation((clientId) =>
        {
            Console.WriteLine($"Client {clientId} Requested Game Creation");
        });
        server.OnRequestGameJoin((clientId) =>
        {
            Console.WriteLine($"Client {clientId} Requested Game Join");
        });
        server.OnDisconnect((clientId) =>
        {
            Console.WriteLine($"Client {clientId} disconnected");
        });
       /* EventManager.Instance.On(EnumHelper.GetStringAttributeValueFromEnum(EnumEvents.ClientConnected), (data,clientId) =>
        {
            Console.WriteLine($"Client {clientId} connected");
        });
        EventManager.Instance.On(EnumHelper.GetStringAttributeValueFromEnum(EnumEvents.RequestGameCreation), (data,clientId) =>
        {
            Console.WriteLine($"Client {clientId} Requested Game Creation");
        });*/
       await server.Start();}
}