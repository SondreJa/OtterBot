using System.Collections.Generic;
using Newtonsoft.Json;
using OtterBot.Repository;

namespace OtterBot.Models
{
    public class ConfigModel : IEntity
    {
        [JsonProperty("id")]
        public string Id => GuildId.ToString();
        public ulong GuildId { get; set; }
        public ulong? LogChannel { get; set; }
        public ulong? BotChannel { get; set; }
        public ulong? MutedRole { get; set; }
        public Dictionary<int, StrikeAction> StrikeActions { get; set; } = new();

        public ConfigModel()
        {
        }

        public ConfigModel(ulong guildId) : base()
        {
            GuildId = guildId;
        }
    }
}