using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OtterBot.Handlers;

namespace OtterBot.Commands
{
    public class BanModule : ModuleBase<SocketCommandContext>
    {
        private readonly BanHandler banHandler;
        public BanModule(BanHandler banHandler)
        {
            this.banHandler = banHandler;

        }

        [Command("ban")]
        public async Task Ban(SocketGuildUser user, string length = null)
        {
            var response = await banHandler.Ban(Context.Guild, user.Id, length);
            await ReplyAsync(response);
        }

        [Command("unban")]
        public async Task Unban(SocketGuildUser user)
        {
            var response = await banHandler.Unban(Context.Guild, user.Id);
            await ReplyAsync(response);
        }

        [Command("unban")]
        public async Task Unban(ulong userId)
        {
            var response = await banHandler.Unban(Context.Guild, userId);
            await ReplyAsync(response);
        }
    }
}