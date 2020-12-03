using System;
using Newtonsoft.Json;
using OtterBot.Repository;

namespace OtterBot.Models
{
    public class BanModel : IEntity
    {
        [JsonProperty("id")]
        public string Id => $"{GuildId}-{UserId}";
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BannedUntil { get; set; }

        public BanModel()
        {

        }

        public BanModel(ulong guildId, ulong userId) : base()
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
}