using System;
using Newtonsoft.Json;
using OtterBot.Repository;

namespace OtterBot.Models
{
    public class MuteModel : IEntity
    {
        [JsonProperty("id")]
        public string Id => $"{GuildId}-{UserId}";
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public bool IsMuted { get; set; }
        public DateTime? MutedUntil { get; set; }

        public MuteModel()
        {

        }

        public MuteModel(ulong guildId, ulong userId) : base()
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
}