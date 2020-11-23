using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Text;

namespace OtterBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;
        private readonly IServiceProvider services;
        private readonly CustomConfig customConfig;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services, CustomConfig customConfig)
        {
            this.services = services;
            this.client = client;
            this.commands = commands;
            this.customConfig = customConfig;

            client.MessageReceived += HandleCommand;
            client.MessageUpdated += HandleMessageUpdated;
            commands.AddModulesAsync(Assembly.GetEntryAssembly(), services).GetAwaiter().GetResult();
        }

        private async Task HandleCommand(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;
            if (message == null)
            {
                return;
            }

            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            var context = new SocketCommandContext(client, message);

            await commands.ExecuteAsync(context, argPos, services);
        }

        private async Task HandleMessageUpdated(Cacheable<IMessage, ulong> cachedMessage, SocketMessage socketmessage, ISocketMessageChannel channel)
        {
            var old = cachedMessage.HasValue ? cachedMessage.Value : (await cachedMessage.DownloadAsync());
            var oldMessage = old.ToString();
            var newMessage = socketmessage.ToString();
            var channelName = channel.Name;

            var user = socketmessage.Author;
            var channels = client.GetGuild(customConfig.GuildId).Channels.First(c => c.Name == customConfig.LogChannel) as IMessageChannel;
            var sb = new StringBuilder();
            sb.AppendLine($"`[{DateTime.UtcNow.ToString("HH:mm:ss")}]` **{user.Username}**#{user.Discriminator} (ID: {user.Id})'s message has been edited in <#{channel.Id}>:");
            sb.Append($">>> ");
            sb.AppendLine($"**From:** {oldMessage}");
            sb.AppendLine($"**To:** {newMessage}");
            await channels.SendMessageAsync(sb.ToString());
        }
    }
}