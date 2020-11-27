using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OtterBot.Models;

namespace OtterBot.Repository
{
    public class MuteRepo
    {
        private readonly ICosmos<MuteModel> cosmos;
        public MuteRepo(ICosmos<MuteModel> cosmos)
        {
            this.cosmos = cosmos;
        }

        public async Task<MuteModel> GetUser(ulong guildId, ulong userId)
        {
            var user = await cosmos.Get($"{guildId}-{userId}");
            if (user == null)
            {
                user = new(guildId, userId);
                await cosmos.Upsert(user);
            }
            return user;
        }

        public async Task Mute(ulong guildId, ulong userId, TimeSpan? length)
        {
            var user = await GetUser(guildId, userId);
            user.IsMuted = true;
            user.MutedUntil = length.HasValue ? DateTime.UtcNow.Add(length.Value) : null;
            await cosmos.Upsert(user);
        }

        public async Task Unmute(ulong guildId, ulong userId)
        {
            var user = await GetUser(guildId, userId);
            user.IsMuted = false;
            user.MutedUntil = null;
            await cosmos.Upsert(user);
        }

        public async Task<IEnumerable<MuteModel>> GetExpiredMutes(ulong guildId)
        {
            var now = DateTime.UtcNow;
            return await cosmos.GetMany(m => m.IsMuted && m.MutedUntil != null && m.MutedUntil < now);
        }
    }
}