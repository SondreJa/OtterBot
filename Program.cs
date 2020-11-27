using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using OtterBot.Handlers;
using SimpleInjector;

namespace OtterBot
{
    class Program
    {
        private static DiscordSocketClient client;
        private static Container container = new();

        static async Task Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", true, true)
                                .AddJsonFile("appsettings.local.json", true, true);

                var config = builder.Build();

                client = await Initialiser.InitialiseDiscordClient(config);
                Initialiser.InitialiseContainer(client, new(), container, config);

                while (true)
                {
                    var handler = container.GetInstance<ScheduleHandler>();
                    await handler.RunScheduledTasks();
                    await Task.Delay(60_000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unhandled exception: {e.Message}", e);
            }
        }
    }
}
