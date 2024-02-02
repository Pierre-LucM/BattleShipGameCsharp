using System.Collections.Concurrent;

namespace BattleShipEventManager;

public class EventManager
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<Action<string, string>>> _eventListeners = new();
    private static EventManager _eventManager;
    public static EventManager Instance
    {
        get
        {
            if(_eventManager==null) _eventManager=new EventManager();
            return _eventManager;
        }
    }

    public void On(string eventName, Action<string, string> callback)
    {
        _eventListeners.GetOrAdd(eventName, _ => new ConcurrentBag<Action<string, string>>()).Add(callback);// add callback to the list of callbacks for the event thread safe
    }

    public void Emit(string eventName, Object data, string clientId)
    {
        if (_eventListeners.TryGetValue(eventName, out var listeners))
            foreach (var listener in listeners)
                listener.Invoke(data.ToString(), clientId);
    }
}