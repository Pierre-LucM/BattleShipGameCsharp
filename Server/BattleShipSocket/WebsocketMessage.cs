using BattleShipSocket.Enum;
using BattleShipSocket.Interfaces;

namespace BattleShipSocket;

public class WebsocketMessage
{
    public Object Data { get;  }

    public string EventName { get; }
    public WebsocketMessage(string eventName, Object data)
    {
        EventName = EnumHelper.GetStringAttributeValueFromEnum(EnumHelper.GetEnumValueFromStringValue<EnumEvents>(eventName));
        Data = data;
    }
 
    
}

