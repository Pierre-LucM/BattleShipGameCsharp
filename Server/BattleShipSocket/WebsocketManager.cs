using System.Collections.Concurrent;
using BattleShipEventManager;
using BattleShipSocket.Enum;
using BattleShipSocket.Typings;

namespace BattleShipSocket;

public class WebsocketManager
{
    private Websocket _websocket;
    private static WebsocketManager? s_instance;
    private EventManager _eventManager;

    private const ushort _port = 9001;
    private string _url = "http://localhost";

    public string Url
    {
        get => _url;
    }

    private List<Room> _rooms = new List<Room>();
    private ConcurrentDictionary<string, Room> _roomClients = new ConcurrentDictionary<string, Room>();

    public ConcurrentDictionary<string, Room> RoomClients
    {
        get => _roomClients;
    }

    public static WebsocketManager WebsocketManagerInstance
    {
        get { return s_instance ?? (s_instance = new WebsocketManager()); }
    }

    private WebsocketManager()
    {
        _websocket = new Websocket(_url, _port);
        _eventManager = EventManager.Instance;
    }

    public void OnConnection(Action<string> callback)
    {
        _eventManager.On(EnumHelper.GetStringAttributeValueFromEnum(EnumEvents.ClientConnected),
            (data, clientId) => { callback(clientId); });
    }

    public void OnRequestGameCreation(Action<string> callback)
    {
        _eventManager.On(EnumHelper.GetStringAttributeValueFromEnum(EnumEvents.RequestGameCreation), (data, clientId) =>
        {
            // TODO: Implement game creation (new BattleShipGame) and add the client to the game
            // TODO: Create a room with the client that requested the game
            Guid roomId = Guid.NewGuid();
            Room room = new Room(roomId, "Room_" + roomId);
            bool isRoomExist = this._roomClients.TryAdd(roomId.ToString(), room);
            if (!isRoomExist)
            {
                throw new Exception("Room already exist");
            }

            _rooms.Add(room);
            room.AddClient(_websocket.Clients[clientId]);
            _websocket.Clients[clientId].RoomId = roomId;
            _websocket.Emit(EnumEvents.GameCreated, $"Game Created Successfully RoomID:{roomId}", clientId);
            callback(clientId);
        });
    }

    public void OnRequestGameJoin(Action<string> callback)
    {
        _eventManager.On(EnumHelper.GetStringAttributeValueFromEnum(EnumEvents.RequestGameJoin),
            (data, clientId) =>
            {
                Console.WriteLine(data);
                JoinGameData joinGameData = Newtonsoft.Json.JsonConvert.DeserializeObject<JoinGameData>(data)!;
                Console.WriteLine(joinGameData.RoomId);
                bool isClientExist = this._websocket.Clients.TryGetValue(clientId, out var client);
                if (client.RoomId != Guid.Empty)
                {
                    throw new Exception("Client already in a room");
                }

                if (!isClientExist)
                {
                    throw new Exception("Client not found");
                }

                Guid.TryParse(joinGameData.RoomId, out Guid roomId);

                bool isRoomExist = this._roomClients.TryGetValue(roomId.ToString(), out var room);
                if (!isRoomExist)
                {
                    throw new Exception("Room not found");
                }

                room!.AddClient(client);
                this._roomClients.TryUpdate(roomId.ToString(), room,
                    this._roomClients.GetOrAdd(roomId.ToString(), room));
                client.RoomId = roomId;
                this._websocket.EmitToRoom(EnumEvents.GameJoined, $"User {client.ClientId} joined the room", room);
                callback(clientId);
            });
    }

    public void OnDisconnect(Action<string> callback)
    {
        _eventManager.On(EnumHelper.GetStringAttributeValueFromEnum(EnumEvents.ClientDisconnected),
            (data, clientId) =>
            {
                bool isClient = this._websocket.Clients.TryGetValue(clientId, out var client);
                if (!isClient)
                {
                    throw new Exception("Client does not exist or is not connected");
                }

                bool isClientInRoom = this._roomClients.TryGetValue(client!.RoomId.ToString(), out var room);
                if (isClientInRoom)
                {
                    room!.RemoveClient(client);
                    this._roomClients.TryUpdate(client!.RoomId.ToString(), room,
                        this._roomClients.GetOrAdd(client!.RoomId.ToString(), room));
                    this._roomClients.TryGetValue(client.RoomId.ToString(), out room);
                    Console.WriteLine(room.Clients.Count);
                    this._websocket.EmitToRoom(EnumEvents.ClientDisconnected, $"User {clientId} left the room", room);
                }

                if (room!.Clients.Count == 0)
                {
                    this._rooms.Remove(room);
                }

                callback(clientId);
            });
    }

    public async Task Start()
    {
        await _websocket.Start();
    }
}