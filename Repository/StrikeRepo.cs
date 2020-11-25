using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using Newtonsoft.Json;
using OtterBot.Models;

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
            var metadata = new PardonMetadata
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
}