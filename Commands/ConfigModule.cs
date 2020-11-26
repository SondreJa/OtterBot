using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using OtterBot.Models;
using OtterBot.Repository;

namespace OtterBot.Commands
{
    public class ConfigModule : ModuleBase<SocketCommandContext>
    {
        private const string SpanPattern = @"(\d+)([wdhms])";
        private static readonly string[] ValidTimeIntervals = new[] { "w", "d", "h", "m", "s" };
        private readonly ConfigRepo repo;
        public ConfigModule(ConfigRepo repo)
        {
            this.repo = repo;
        }

        [Command("setlogchannel")]
        public async Task SetLogChannel()
        {
            await repo.SetLogChannel(Context.Guild.Id, Context.Channel.Id);
            await ReplyAsync("Log channel set");
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
            if (!TryParseToSpan(length, out var span))
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

        private bool TryParseToSpan(string length, out TimeSpan? span)
        {
            span = null;
            if (length != null)
            {
                if (!Regex.IsMatch(length, SpanPattern))
                {
                    return false;
                }
                span = new TimeSpan();
                var matches = Regex.Matches(length, SpanPattern);
                foreach (Match match in matches)
                {
                    var captures = match.Groups;
                    var intervalInt = int.Parse(captures[1].Value);
                    var addSpan = SwitchOnInterval(intervalInt, captures[2].Value);
                    span = span.Value.Add(addSpan);
                }
            }
            return true;
        }

        private TimeSpan SwitchOnInterval(int length, string interval) =>
            interval switch
            {
                "w" => new TimeSpan((length * 7), 0, 0, 0),
                "d" => new TimeSpan(length, 0, 0, 0),
                "h" => new TimeSpan(length, 0, 0),
                "m" => new TimeSpan(0, length, 0),
                "s" => new TimeSpan(0, 0, length),
                _ => throw new ArgumentException("Invalid interval passed")
            };
    }
}