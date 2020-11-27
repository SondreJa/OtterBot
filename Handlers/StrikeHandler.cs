using System;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using OtterBot.Models;
using OtterBot.Repository;
using OtterBot.Utility;

namespace OtterBot.Handlers
{
    public class StrikeHandler
    {
        private readonly ConfigRepo configRepo;
        public StrikeHandler(ConfigRepo configRepo)
        {
            this.configRepo = configRepo;
        }

        public async Task<string> HandleStrike(SocketGuild guild, SocketGuildUser user, StrikeAction action)
        {
            switch (action.Action)
            {
                case BotAction.Mute:
                    var mutedRole = await configRepo.GetMutedRole(guild.Id);
                    if (mutedRole == null)
                    {
                        return $"No muted role configured";
                    }
                    await user.AddRoleAsync(guild.GetRole(mutedRole.Value));
                    return $"Muting {Formatter.FullName(user, true)}{(action.Length.HasValue ? $" for{Formatter.TimespanToString(action.Length.Value)}" : string.Empty)}.";
                case BotAction.Kick:
                    return $"Kicking {Formatter.FullName(user, true)}.";
                case BotAction.Ban:
                    return $"Banning {Formatter.FullName(user, true)}{(action.Length.HasValue ? $" for{Formatter.TimespanToString(action.Length.Value)}" : string.Empty)}.";
                case BotAction.Nothing:
                default:
                    return string.Empty;
            }
        }
    }
}