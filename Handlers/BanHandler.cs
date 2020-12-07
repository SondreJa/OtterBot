using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using OtterBot.Repository;
using OtterBot.Utility;

namespace OtterBot.Handlers
{
    public class BanHandler
    {
        private readonly ConfigRepo configRepo;
        private readonly BanRepo banRepo;
        private readonly DiscordSocketClient client;

        public BanHandler(ConfigRepo configRepo, BanRepo banRepo, DiscordSocketClient client)
        {
            this.client = client;
            this.banRepo = banRepo;
            this.configRepo = configRepo;
        }

        public async Task<string> Ban(SocketGuild guild, ulong userId, string length = null)
        {
            TimeSpan? span = null;
            if (length != null && !Parser.TryParseToSpan(length, out span))
            {
                return $"Unable to parse length of ban from {length}";
            }

            return await Ban(guild, userId, span);
        }

        public async Task<string> Ban(SocketGuild guild, ulong userId, TimeSpan? length = null)
        {
            await banRepo.Ban(guild.Id, userId, length);
            await guild.AddBanAsync(userId);
            var user = guild.GetUser(userId);
            return $"{Formatter.FullName(user, true)} banned{(length.HasValue ? $" for{Formatter.TimespanToString(length.Value)}" : string.Empty)}.";
        }

        public async Task<string> Unban(SocketGuild guild, ulong userId)
        {
            var ban = banRepo.GetUser(guild.Id, userId);
            if (ban != null)
            {
                await guild.RemoveBanAsync(userId);
                await banRepo.Unban(guild.Id, userId);
                var user = guild.GetUser(userId);
                return user == null ? $"ID: {userId} unbanned." : $"{Formatter.FullName(user, true)} unbanned.";
            }
            else
            {
                var user = client.GetUser(userId);
                return $"{Formatter.FullName(user, true)} is not banned.";
            }
        }

        public async Task<IEnumerable<ulong>> GetExpiredBans(SocketGuild guild)
        {
            var results = await banRepo.GetExpiredBans(guild.Id);
            return results.Select(u => u.UserId);
        }
    }
}