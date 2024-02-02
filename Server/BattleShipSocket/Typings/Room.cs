namespace BattleShipSocket.Typings;

public class Room :IDisposable
{
    private Guid _roomId;
    private List<Client> _clients;
    private string? _roomName;
    private bool _isPrivate;
    private string? _password;

    public Guid RoomId
    {
        get => _roomId;
    }
    public List<Client> Clients
    {
        get => _clients;
    }
    public string? RoomName
    {
        get => _roomName;
    }
    public bool IsPrivate
    {
        get => _isPrivate;
    }
    public string? Password
    {
        get => _password;
    }

    public override string ToString()
    {
        return _roomId.ToString();
    }

    public Room(Guid roomId, string? roomName, bool isPrivate=false, string? password=null)
    {
        _roomId = roomId;
        _roomName = roomName;
        _isPrivate = isPrivate;
        _password = password;
        _clients = new List<Client>();
    }
    public void AddClient(Client client)
    {
        _clients.Add(client);
    }
    public void RemoveClient(Client client)
    {
        _clients.Remove(client);
    }
    public void Dispose()
    {
        _clients.Clear();
    }
}