using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using OtterBot.Utility;

namespace OtterBot.Commands
{
    public class KickModule : ModuleBase<SocketCommandContext>
    {
        [Command("kick")]
        public async Task Ban(SocketGuildUser user, [Remainder] string reason = null)
        {
            await user.KickAsync(reason);
            var response = $"{Formatter.FullName(user, true)} has been kicked.{(reason != null ? $" Reason: {reason}" : string.Empty)}";
            await ReplyAsync(response);
        }
    }
}