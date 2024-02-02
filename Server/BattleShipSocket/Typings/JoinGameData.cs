namespace BattleShipSocket.Typings;

public class JoinGameData
{
    public string RoomId { get; }
    public string? Password { get; set; }
    public string? RoomName { get; set; }
    public bool IsPrivate { get; set; }
    public JoinGameData(string roomId, string? password, string? roomName, bool isPrivate)
    {
        RoomId = roomId;
        Password = password;
        RoomName = roomName;
        IsPrivate = isPrivate;
    }
}