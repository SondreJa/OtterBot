using System.Threading.Tasks;
using Discord.WebSocket;
using OtterBot.Repository;
using Discord;
using Dasync.Collections;

namespace OtterBot.Handlers
{
    public class ScheduleHandler
    {
        private readonly MuteHandler muteHandler;
        private readonly ConfigRepo configRepo;
        private readonly DiscordSocketClient client;

        public ScheduleHandler(DiscordSocketClient client, ConfigRepo configRepo, MuteHandler muteHandler)
        {
            this.client = client;
            this.configRepo = configRepo;
            this.muteHandler = muteHandler;
        }

        public async Task RunScheduledTasks()
        {
            var guilds = await configRepo.GetAllGuilds();
            await guilds.ParallelForEachAsync(async guild =>
            {
                await RunScheduledTasks(guild);
            });
        }

        public async Task RunScheduledTasks(ulong guildId)
        {
            var guild = client.GetGuild(guildId);
            if (guild == null)
            {
                return;
            }

            await guild.SyncPromise;
            var botChannelId = await configRepo.GetBotChannel(guildId);
            if (!botChannelId.HasValue)
            {
                return;
            }
            var botChannel = guild.GetChannel(botChannelId.Value) as IMessageChannel;
            if (botChannel == null)
            {
                return;
            }
            await UnmuteExpiredMutes(guild, botChannel);
        }

        private async Task UnmuteExpiredMutes(SocketGuild guild, IMessageChannel botChannel)
        {
            var expired = await muteHandler.GetExpiredMutes(guild);
            foreach (var user in expired)
            {
                var result = await muteHandler.Unmute(guild, user);
                await botChannel.SendMessageAsync(result);
            }
        }
    }
}