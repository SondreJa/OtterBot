using System.Threading.Tasks;
using OtterBot.Models;

namespace OtterBot.Repository
{
    public class ConfigRepo
    {
        private readonly ICosmos<ConfigModel> cosmos;

        public ConfigRepo(ICosmos<ConfigModel> cosmos)
        {
            this.cosmos = cosmos;
        }

        public async Task SetLogChannel(ulong guildId, ulong logChannelId)
        {
            var config = await GetServerConfig(guildId);
            config.LogChannel = logChannelId;
            await cosmos.Upsert(config);
        }

        public ulong GetLogChannel(ulong guildId) => cosmos.Get(guildId.ToString()).GetAwaiter().GetResult().LogChannel;

        private async Task<ConfigModel> GetServerConfig(ulong guildId)
        {
            var config = await cosmos.Get(guildId.ToString());
            if (config == null)
            {
                config = new(guildId);
            }
            return config;
        }
    }
}