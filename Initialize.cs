using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using System;
using System.Threading.Tasks;

namespace OtterBot
{
    public class Initialize
    {
        public static async Task<DiscordSocketClient> GetClient(IConfigurationRoot config)
        {
            var token = config["Token"];
            var discordConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
            var client = new DiscordSocketClient(discordConfig);
            client.Log += Log;
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            return client;
        }

        public static IServiceProvider BuildServiceProvider(DiscordSocketClient client, CommandService commands, Container container, CustomConfig customConfig)
        {
            var provider = new ServiceCollection()
                    .AddSimpleInjector(container)
                    .BuildServiceProvider();

            container.RegisterInstance(client);
            container.RegisterInstance(commands);
            container.RegisterSingleton<CommandHandler>();
            container.RegisterInstance<IServiceProvider>(provider);
            container.RegisterInstance(customConfig);

            provider.UseSimpleInjector(container);
            container.Verify();

            return provider;
        }

        private static Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}