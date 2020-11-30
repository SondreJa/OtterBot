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

        public UserHandler(DiscordSocketClient client, ConfigRepo configRepo)
        {
            this.configRepo = configRepo;
            this.client = client;

            client.UserJoined += UserJoined;
            client.UserLeft += UserLeft;
        }

        private async Task UserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var logChannelId = await configRepo.GetLogChannel(guild.Id);
            if (logChannelId == null)
            {
                return;
            }
            var logChannel = guild.GetChannel(logChannelId.Value) as IMessageChannel;
            var span = DateTime.UtcNow - user.CreatedAt;
            var sb = new StringBuilder();
            sb.AppendLine($"{Formatter.NowBlock()} {Emotes.Inbox} {Formatter.FullName(user, true)} joined the server.");
            sb.AppendLine($"Creation: {user.CreatedAt.ToString("r")} ({Formatter.TimespanToString(span)} ago)");
            await logChannel.SendMessageAsync(sb.ToString());
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            var guild = user.Guild;
            var logChannelId = await configRepo.GetLogChannel(guild.Id);
            if (logChannelId == null)
            {
                return;
            }
            var logChannel = guild.GetChannel(logChannelId.Value) as IMessageChannel;
            var sb = new StringBuilder();
            sb.AppendLine($"{Formatter.NowBlock()} {Emotes.Outbox} {Formatter.FullName(user, true)} left or was kicked from the server.");
            if (user.JoinedAt.HasValue)
            {
                var span = DateTime.UtcNow - user.JoinedAt.Value;
                sb.AppendLine($"Joined: {user.JoinedAt.Value.ToString("r")} ({Formatter.TimespanToString(span)} ago)");
            }
            await logChannel.SendMessageAsync(sb.ToString());
        }
    }
}