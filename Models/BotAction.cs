using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OtterBot.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BotAction
    {
        Nothing,
        Mute,
        Kick,
        Ban
    }
}