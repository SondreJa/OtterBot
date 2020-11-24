using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OtterBot.Repository
{
    public class StrikeRepo
    {
        private readonly ICosmos<StrikeModel> cosmos;
        public StrikeRepo(ICosmos<StrikeModel> cosmos)
        {
            this.cosmos = cosmos;
        }

        public async Task<StrikeModel> GetUser(ulong guildId, ulong userId)
        {
            var user = await cosmos.Get($"{guildId}-{userId}");
            if (user == null)
            {
                user = new(guildId, userId);
            }
            return user;
        }

        public async Task<int> AddStrikes(ulong guildId, ulong userId, ulong givenBy, int strikes, string reason)
        {
            var user = await GetUser(guildId, userId);
            user.Strikes += strikes;
            var metadata = new StrikeMetadata
            {
                Id = Guid.NewGuid().ToString(),
                Amount = strikes,
                Reason = reason,
                GivenBy = givenBy,
                Timestamp = DateTime.UtcNow
            };
            user.StrikeMetadata.Add(metadata);
            await cosmos.Upsert(user);
            return user.Strikes;
        }

        public async Task<int> RemoveStrikes(ulong guildId, ulong userId, ulong givenBy, int strikes, string reason)
        {
            var user = await GetUser(guildId, userId);
            user.Strikes = Math.Max(0, user.Strikes - strikes);
            var metadata = new StrikeMetadata
            {
                Id = Guid.NewGuid().ToString(),
                Amount = strikes,
                Reason = reason,
                GivenBy = givenBy,
                Timestamp = DateTime.UtcNow
            };
            user.PardonMetadata.Add(metadata);
            await cosmos.Upsert(user);
            return user.Strikes;
        }
    }

    public class StrikeModel : IEntity
    {
        [JsonProperty("id")]
        public string Id => $"{GuildId}-{UserId}";
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public int Strikes { get; set; }
        public List<StrikeMetadata> StrikeMetadata { get; set; } = new();
        public List<StrikeMetadata> PardonMetadata { get; set; } = new();

        public StrikeModel()
        {

        }

        public StrikeModel(ulong guildId, ulong userId) : base()
        {
            GuildId = guildId;
            UserId = userId;
        }
    }

    public class StrikeMetadata
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; }
        public ulong GivenBy { get; set; }
        public DateTime Timestamp { get; set; }
    }
}