using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OtterBot.Models;

namespace OtterBot.Repository
{
    public class BanRepo
    {
        private readonly ICosmos<BanModel> cosmos;
        public BanRepo(ICosmos<BanModel> cosmos)
        {
            this.cosmos = cosmos;
        }

        public async Task<BanModel> GetUser(ulong guildId, ulong userId)
        {
            var user = await cosmos.Get($"{guildId}-{userId}");
            if (user == null)
            {
                user = new(guildId, userId);
                await cosmos.Upsert(user);
            }
            return user;
        }

        public async Task Ban(ulong guildId, ulong userId, TimeSpan? length)
        {
            var user = await GetUser(guildId, userId);
            user.IsBanned = true;
            user.BannedUntil = length.HasValue ? DateTime.UtcNow.Add(length.Value) : null;
            await cosmos.Upsert(user);
        }

        public async Task Unban(ulong guildId, ulong userId)
        {
            var user = await GetUser(guildId, userId);
            user.IsBanned = false;
            user.BannedUntil = null;
            await cosmos.Upsert(user);
        }

        public async Task<IEnumerable<BanModel>> GetExpiredBans(ulong guildId)
        {
            var now = DateTime.UtcNow;
            return await cosmos.GetMany(m => m.IsBanned && m.BannedUntil != null && m.BannedUntil < now);
        }
    }
}