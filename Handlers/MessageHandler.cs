using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OtterBot.Constants;
using OtterBot.Repository;
using SimpleInjector;

namespace OtterBot.Handlers
{
    public class MessageHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;
        private readonly IServiceProvider services;
        private readonly ConfigRepo configRepo;

        public MessageHandler(DiscordSocketClient client, CommandService commands, ConfigRepo configRepo, Container container)
        {
            this.services = container;
            this.client = client;
            this.commands = commands;
            this.configRepo = configRepo;

            client.MessageReceived += HandleCommand;
            client.MessageUpdated += HandleMessageUpdated;
            client.MessageDeleted += HandleMessageDeleted;
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

        private async Task HandleMessageUpdated(Cacheable<IMessage, ulong> cachedMessage, SocketMessage socketMessage, ISocketMessageChannel channel)
        {
            if (socketMessage.Author.IsBot)
            {
                return;
            }

            var old = cachedMessage.HasValue ? cachedMessage.Value : (await cachedMessage.DownloadAsync());
            var oldMessage = old.ToString();
            var newMessage = socketMessage.ToString();

            var context = new SocketCommandContext(client, socketMessage as SocketUserMessage);
            var logChannelId = await configRepo.GetLogChannel(context.Guild.Id);
            var logChannel = context.Guild.GetChannel(logChannelId) as IMessageChannel;

            var embed = new EmbedBuilder();
            embed.Color = Color.Gold;
            embed.WithDescription($"**From**: {oldMessage}\n**To**: {newMessage}");

            var user = socketMessage.Author;
            var text = $"`[{DateTime.UtcNow.ToString("HH:mm:ss")}]` {Emotes.Warning} **{user.Username}**#{user.Discriminator} (ID: {user.Id})'s message has been edited in <#{channel.Id}>:";

            await logChannel.SendMessageAsync(text: text, embed: embed.Build());
        }

        private async Task HandleMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, ISocketMessageChannel channel)
        {
            var old = cachedMessage.HasValue ? cachedMessage.Value : (await cachedMessage.DownloadAsync());
            if (old.Author.IsBot)
            {
                return;
            }

            var context = new SocketCommandContext(client, old as SocketUserMessage);
            var logChannelId = await configRepo.GetLogChannel(context.Guild.Id);
            var logChannel = context.Guild.GetChannel(logChannelId) as IMessageChannel;

            var embed = new EmbedBuilder();
            embed.Color = Color.Red;
            embed.WithDescription($"**Deleted**: {old.ToString()}");

            var user = old.Author;
            var text = $"`[{DateTime.UtcNow.ToString("HH:mm:ss")}]` {Emotes.Cross} **{user.Username}**#{user.Discriminator} (ID: {user.Id})'s message has been deleted from <#{channel.Id}>:";

            await logChannel.SendMessageAsync(text: text, embed: embed.Build());
        }
    }
}