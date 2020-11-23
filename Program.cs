using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using SimpleInjector;

namespace OtterBot
{
    class Program
    {
        private static DiscordSocketClient client;
        private static Container container;

        static async Task Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", true, true)
                                .AddJsonFile("appsettings.local.json", true, true);

                var config = builder.Build();
                var customConfig = new CustomConfig
                {
                    GuildId = config.GetValue<ulong>("GuildId"),
                    LogChannel = config["LogChannel"]
                };

                client = await Initialize.GetClient(config);
                container = new Container();
                Initialize.BuildServiceProvider(client, new(), container, customConfig);

                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unhandled exception: {e.Message}", e);
            }
        }
    }
}
