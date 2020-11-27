using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using OtterBot.Repository;
using OtterBot.Utility;

namespace OtterBot.Handlers
{
    public class MuteHandler
    {
        private readonly ConfigRepo configRepo;
        private readonly MuteRepo muteRepo;

        public MuteHandler(ConfigRepo configRepo, MuteRepo muteRepo)
        {
            this.muteRepo = muteRepo;
            this.configRepo = configRepo;
        }

        public async Task<string> Mute(SocketGuild guild, SocketGuildUser user, string length = null)
        {
            var mutedRoleId = await configRepo.GetMutedRole(guild.Id);
            var mutedRole = mutedRoleId.HasValue ? guild.GetRole(mutedRoleId.Value) : null;
            if (mutedRole == null)
            {
                return "No muted role configured.";
            }

            TimeSpan? span = null;
            if (length != null && !Parser.TryParseToSpan(length, out span))
            {
                return $"Unable to parse length of mute from {length}";
            }

            await user.AddRoleAsync(mutedRole);
            await muteRepo.Mute(guild.Id, user.Id, span);
            return $"{Formatter.FullName(user, true)} muted{(span.HasValue ? $" for{Formatter.TimespanToString(span.Value)}" : string.Empty)}.";
        }

        public async Task<string> Unmute(SocketGuild guild, SocketGuildUser user)
        {
            var mutedRole = await configRepo.GetMutedRole(guild.Id);
            if (mutedRole == null)
            {
                return "No muted role configured, no action taken.";
            }
            if (user.Roles.Any(r => r.Id == mutedRole.Value))
            {
                await user.RemoveRoleAsync(guild.GetRole(mutedRole.Value));
                await muteRepo.Unmute(guild.Id, user.Id);
                return $"{Formatter.FullName(user, true)} unmuted.";
            }
            else
            {
                return $"{Formatter.FullName(user, true)} is not muted.";
            }
        }

        public async Task<IEnumerable<SocketGuildUser>> GetExpiredMutes(SocketGuild guild)
        {
            var results = await muteRepo.GetExpiredMutes(guild.Id);
            var userIds = results.Select(u => u.UserId);
            return guild.Users.Where(u => userIds.Contains(u.Id));
        }
    }
}