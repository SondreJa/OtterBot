using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using OtterBot.Constants;
using OtterBot.Repository;
using OtterBot.Utility;

namespace OtterBot.Handlers
{
    public class UserHandler
    {
        private readonly DiscordSocketClient client;
        private readonly ConfigRepo configRepo;
        private readonly BanRepo banRepo;

        public UserHandler(DiscordSocketClient client, ConfigRepo configRepo, BanRepo banRepo)
        {
            this.banRepo = banRepo;
            this.configRepo = configRepo;
            this.client = client;

            client.UserJoined += UserJoined;
            client.UserLeft += UserLeft;
            client.UserBanned += UserBanned;
        }

        private async Task UserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var (foundChannel, logChannel) = await TryGetLogChannel(guild);
            if (!foundChannel)
            {
                return;
            }
            var span = DateTime.UtcNow - user.CreatedAt;
            var sb = new StringBuilder();
            sb.AppendLine($"{Formatter.NowBlock()} {Emotes.Inbox} {Formatter.FullName(user, true)} joined the server.");
            sb.AppendLine($"Creation: {user.CreatedAt.ToString("r")} ({Formatter.TimespanToString(span)} ago)");
            await logChannel.SendMessageAsync(sb.ToString());
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            if ((await banRepo.GetUser(user.Guild.Id, user.Id)) != null)
            {
                return;
            }
            var guild = user.Guild;
            var (foundChannel, logChannel) = await TryGetLogChannel(guild);
            if (!foundChannel)
            {
                return;
            }
            var sb = new StringBuilder();
            sb.AppendLine($"{Formatter.NowBlock()} {Emotes.Outbox} {Formatter.FullName(user, true)} left or was kicked from the server.");
            if (user.JoinedAt.HasValue)
            {
                var span = DateTime.UtcNow - user.JoinedAt.Value;
                sb.AppendLine($"Joined: {user.JoinedAt.Value.ToString("r")} ({Formatter.TimespanToString(span)} ago)");
            }
            await logChannel.SendMessageAsync(sb.ToString());
        }

        private async Task UserBanned(SocketUser user, SocketGuild guild)
        {
            var (foundChannel, logChannel) = await TryGetLogChannel(guild);
            if (!foundChannel)
            {
                return;
            }
            var sb = new StringBuilder();
            sb.AppendLine($"{Formatter.NowBlock()} {Emotes.Hammer} {Formatter.FullName(user, true)} was banned from the server.");
            await logChannel.SendMessageAsync(sb.ToString());
        }

        private async Task<(bool, IMessageChannel)> TryGetLogChannel(SocketGuild guild)
        {
            var logChannelId = await configRepo.GetLogChannel(guild.Id);
            if (logChannelId == null)
            {
                return (false, null);
            }
            var logChannel = guild.GetChannel(logChannelId.Value) as IMessageChannel;
            return (logChannel != null, logChannel);
        }
    }
}