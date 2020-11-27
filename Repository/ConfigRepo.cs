using System.Collections.Generic;
using System.Linq;
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
        public async Task<IEnumerable<ulong>> GetAllGuilds() => (await cosmos.GetMany(_ => true)).Select(c => c.GuildId);
        public async Task<ulong?> GetLogChannel(ulong guildId) => (await cosmos.Get(guildId.ToString())).LogChannel;
        public async Task<ulong?> GetBotChannel(ulong guildId) => (await cosmos.Get(guildId.ToString())).BotChannel;
        public async Task<ulong?> GetMutedRole(ulong guildId) => (await cosmos.Get(guildId.ToString())).MutedRole;

        public async Task<StrikeAction> GetStrikeAction(ulong guildId, int oldStrikes, int strikes)
        {
            var config = await cosmos.Get(guildId.ToString());
            if (config.StrikeActions.ContainsKey(strikes) && config.StrikeActions[strikes].Action != BotAction.Nothing)
            {
                return config.StrikeActions[strikes];
            }
            var possibleActions = config.StrikeActions.Where(kvp => kvp.Key > oldStrikes && kvp.Key < strikes);
            if (!possibleActions.Any())
            {
                return new StrikeAction { Action = BotAction.Nothing };
            }
            foreach (var action in possibleActions.OrderByDescending(kvp => kvp.Key))
            {
                if (action.Value.Action != BotAction.Nothing)
                {
                    return action.Value;
                }
            }
            return new StrikeAction { Action = BotAction.Nothing };
        }

        public async Task SetLogChannel(ulong guildId, ulong logChannelId)
        {
            var config = await GetServerConfig(guildId);
            config.LogChannel = logChannelId;
            await cosmos.Upsert(config);
        }

        public async Task SetBotChannel(ulong guildId, ulong botChannelId)
        {
            var config = await GetServerConfig(guildId);
            config.BotChannel = botChannelId;
            await cosmos.Upsert(config);
        }

        public async Task SetMutedRole(ulong guildId, ulong mutedRole)
        {
            var config = await GetServerConfig(guildId);
            config.MutedRole = mutedRole;
            await cosmos.Upsert(config);
        }

        public async Task SetStrikeAction(ulong guildId, int amount, StrikeAction action)
        {
            var config = await GetServerConfig(guildId);
            config.StrikeActions[amount] = action;
            await cosmos.Upsert(config);
        }

        public async Task RemoveStrikeAction(ulong guildId, int amount)
        {
            var config = await GetServerConfig(guildId);
            config.StrikeActions.Remove(amount);
            await cosmos.Upsert(config);
        }

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