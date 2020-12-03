using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using OtterBot.Repository;
using OtterBot.Handlers;
using SimpleInjector;
using System;
using System.Threading.Tasks;

namespace OtterBot
{
    public class Initialiser
    {
        public static async Task<DiscordSocketClient> InitialiseDiscordClient(IConfigurationRoot config)
        {
            var token = config["Token"];
            var discordConfig = new DiscordSocketConfig { MessageCacheSize = 500, AlwaysDownloadUsers = true };
            var client = new DiscordSocketClient(discordConfig);
            client.Log += Log;
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            return client;
        }

        public static IServiceProvider InitialiseContainer(DiscordSocketClient client, CommandService commands, Container container, IConfigurationRoot config)
        {
            container.RegisterInstance(client);
            container.RegisterInstance(commands);
            container.RegisterInstance(config);

            container.RegisterSingleton<ConfigRepo>();
            container.RegisterSingleton<StrikeRepo>();
            container.RegisterSingleton<MuteRepo>();
            container.RegisterSingleton<BanRepo>();

            container.RegisterSingleton<MessageHandler>();
            container.RegisterSingleton<StrikeHandler>();
            container.RegisterSingleton<MuteHandler>();
            container.RegisterSingleton<ScheduleHandler>();
            container.RegisterSingleton<UserHandler>();
            container.RegisterSingleton<BanHandler>();

            container.RegisterSingleton(typeof(ICosmos<>), typeof(Cosmos<>));

            container.Verify();
            return container;
        }

        // Do something proper with the logs in the future
        private static Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}