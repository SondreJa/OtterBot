using System;
using System.Collections.Generic;
using Discord.WebSocket;
using Newtonsoft.Json;
using OtterBot.Repository;

namespace OtterBot.Models
{
    public class StrikeModel : IEntity
    {
        [JsonProperty("id")]
        public string Id => $"{GuildId}-{UserId}";
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public int Strikes { get; set; }
        public List<StrikeMetadata> StrikeMetadata { get; set; } = new();
        public List<PardonMetadata> PardonMetadata { get; set; } = new();

        public StrikeModel()
        {

        }

        public StrikeModel(ulong guildId, ulong userId) : base()
        {
            GuildId = guildId;
            UserId = userId;
        }
    }

    public abstract class HistoryMetadata
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; }
        public ulong GivenBy { get; set; }
        public DateTime Timestamp { get; set; }

        public abstract string ToString(SocketUser user);
    }

    public class StrikeMetadata : HistoryMetadata
    {

        public override string ToString(SocketUser striker)
        {
            string givenBy = striker != null ? $"by **{striker.Username}**#{striker.Discriminator} " : string.Empty;
            return $"`[{Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}]` {Amount} strike(s) given {givenBy}because: {Reason}";
        }
    }

    public class PardonMetadata : HistoryMetadata
    {
        public override string ToString(SocketUser pardoner)
        {
            string givenBy = pardoner != null ? $"by **{pardoner.Username}**#{pardoner.Discriminator} " : string.Empty;
            return $"`[{Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}]` {Amount} strike(s) pardoned {givenBy}because: {Reason}";
        }
    }
}