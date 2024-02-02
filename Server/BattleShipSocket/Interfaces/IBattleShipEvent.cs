using BattleShipSocket.Enum;

namespace BattleShipSocket.Interfaces;

public interface IBattleShipEvent
{
    void On(EnumEvents eventName, Action<string, string> callback);
    Task Emit(EnumEvents eventName, string data, string clientId);

    void EmitToRoom(EnumEvents eventName, string data,
        string roomId); // roomId is the key in a dictionary of rooms where the value is a list of clients

    void
        Broadcast(EnumEvents eventName,
            string data); // Broadcast to all clients in every room (event like server is shutting down or something that needs to be broadcasted to all clients)
}