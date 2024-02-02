using System.Reflection;

namespace BattleShipSocket.Enum;

public class EnumStringValueAttribute(string value) : Attribute
{
    public string Value { get; } = value;
    
}

public static class EnumHelper
{
    public static T GetEnumValueFromStringValue<T>(string stringValue) where T : struct
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }

        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(EnumStringValueAttribute)) is EnumStringValueAttribute attribute)
            {
                if (attribute.Value == stringValue)
                {
                    return (T)field.GetValue(null);
                }
            }
        }

        throw new ArgumentException($"No enum value found for string '{stringValue}' in enum {typeof(T).Name}");
    }
    public static string GetStringAttributeValueFromEnum<T>(T enumValue) where T : struct
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }

        var field = typeof(T).GetField(enumValue.ToString());

        if (field == null)
        {
            throw new ArgumentException($"Enum value {enumValue} not found in enum {typeof(T).Name}");
        }

        var attribute = Attribute.GetCustomAttribute(field, typeof(EnumStringValueAttribute)) as EnumStringValueAttribute;

        if (attribute != null)
        {
            return attribute.Value;
        }

        throw new ArgumentException($"No EnumStringValueAttribute found for enum value {enumValue} in enum {typeof(T).Name}");
    }
}
public enum EnumEvents
{
    [EnumStringValue("request_game_creation")]
    RequestGameCreation,
    [EnumStringValue("game_created")] GameCreated,
    [EnumStringValue("request_game_join")] RequestGameJoin,
    [EnumStringValue("game_joined")] GameJoined,

    [EnumStringValue("request_game_start")]
    RequestGameStart,
    [EnumStringValue("game_started")] GameStarted,

    [EnumStringValue("request_game_leave")]
    RequestGameLeave,
    [EnumStringValue("game_left")] GameLeft,

    [EnumStringValue("request_game_ready")]
    RequestGameReady,
    [EnumStringValue("game_ready")] GameReady,

    [EnumStringValue("request_game_unready")]
    RequestGameUnready,
    [EnumStringValue("game_unready")] GameUnready,
    [EnumStringValue("client_connected")] ClientConnected,
    [EnumStringValue("client_disconnected")] ClientDisconnected,
}