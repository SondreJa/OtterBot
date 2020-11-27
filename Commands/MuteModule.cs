using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using OtterBot.Handlers;

namespace OtterBot.Commands
{
    public class MuteModule : ModuleBase<SocketCommandContext>
    {
        private readonly MuteHandler muteHandler;
        public MuteModule(MuteHandler muteHandler)
        {
            this.muteHandler = muteHandler;
        }

        [Command("mute")]
        public async Task Mute(SocketGuildUser user, string length = null)
        {
            var response = await muteHandler.Mute(Context.Guild, user, length);
            await ReplyAsync(response);
        }

        [Command("unmute")]
        public async Task Unmute(SocketGuildUser user)
        {
            var response = await muteHandler.Unmute(Context.Guild, user);
            await ReplyAsync(response);
        }
    }
}