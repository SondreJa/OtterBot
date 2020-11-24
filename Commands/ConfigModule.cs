using System.Threading.Tasks;
using Discord.Commands;
using OtterBot.Repository;

namespace OtterBot.Commands
{
    public class ConfigModule : ModuleBase<SocketCommandContext>
    {
        private readonly ConfigRepo repo;
        public ConfigModule(ConfigRepo repo)
        {
            this.repo = repo;
        }

        [Command("setlogchannel")]
        public async Task SetLogChannel()
        {
            await repo.SetLogChannel(Context.Guild.Id, Context.Channel.Id);
            await ReplyAsync("Log channel set");
        }
    }
}