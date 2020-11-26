using System.Collections.Generic;
using Newtonsoft.Json;
using OtterBot.Repository;

namespace OtterBot.Models
{
    public class ConfigModel : IEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public ulong LogChannel { get; set; }
        public Dictionary<int, StrikeAction> StrikeActions { get; set; } = new();

        public ConfigModel()
        {
        }

        public ConfigModel(ulong guildId) : base()
        {
            Id = guildId.ToString();
        }
    }
}