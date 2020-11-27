using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using OtterBot.Models;
using OtterBot.Repository;
using OtterBot.Utility;

namespace OtterBot.Commands
{
    public class ConfigModule : ModuleBase<SocketCommandContext>
    {
        private readonly ConfigRepo repo;
        public ConfigModule(ConfigRepo repo)
        {
            this.repo = repo;
        }

        [Command("setlogchannel")]
        public async Task SetLogChannel(SocketTextChannel channel)
        {
            await repo.SetLogChannel(Context.Guild.Id, channel.Id);
            await ReplyAsync("Log channel set");
        }

        [Command("setbotchannel")]
        public async Task SetBotChannel(SocketTextChannel channel)
        {
            await repo.SetBotChannel(Context.Guild.Id, channel.Id);
            await ReplyAsync("Bot channel set");
        }

        [Command("setmutedrole")]
        public async Task SetMutedRole(SocketRole role)
        {
            await repo.SetMutedRole(Context.Guild.Id, role.Id);
            await ReplyAsync("Muted role set");
        }

        [Command("setstrike")]
        public async Task SetStrike(int amount, string action, string length = null)
        {
            if (amount <= 0)
            {
                await ReplyAsync("Amount of strikes must be greater than 0.");
                return;
            }
            if (!Enum.TryParse(action, true, out BotAction botAction))
            {
                await ReplyAsync($"Unable to determine action from '{action}'");
                return;
            }
            if (!Parser.TryParseToSpan(length, out var span))
            {
                await ReplyAsync($"Unable to determine length of action from '{length}'");
                return;
            }
            var strikeAction = new StrikeAction { Action = botAction, Length = span };
            await repo.SetStrikeAction(Context.Guild.Id, amount, strikeAction);
            await ReplyAsync($"Strike action set for {amount} strike(s)");
        }

        [Command("removestrike")]
        public async Task RemoveStrike(int amount)
        {
            await repo.RemoveStrikeAction(Context.Guild.Id, amount);
            await ReplyAsync($"Strike action cleared for {amount} strikes(s)");
        }
    }
}