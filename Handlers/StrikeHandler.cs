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
        private readonly MuteHandler muteHandler;
        private readonly BanHandler banHandler;

        public StrikeHandler(ConfigRepo configRepo, MuteHandler muteHandler, BanHandler banHandler)
        {
            this.banHandler = banHandler;
            this.muteHandler = muteHandler;
            this.configRepo = configRepo;
        }

        public async Task<string> HandleStrike(SocketGuild guild, SocketGuildUser user, StrikeAction action)
        {
            switch (action.Action)
            {
                case BotAction.Mute:
                    return await muteHandler.Mute(guild, user, action.Length);
                case BotAction.Kick:
                    await user.KickAsync();
                    return $"Kicking {Formatter.FullName(user, true)}.";
                case BotAction.Ban:
                    return await banHandler.Ban(guild, user.Id, action.Length);
                case BotAction.Nothing:
                default:
                    return string.Empty;
            }
        }
    }
}