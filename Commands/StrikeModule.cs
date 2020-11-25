using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using OtterBot.Constants;
using OtterBot.Models;
using OtterBot.Repository;
using OtterBot.Utility;

namespace OtterBot.Commands
{
    public class StrikeModule : ModuleBase<SocketCommandContext>
    {
        private readonly StrikeRepo strikeRepo;
        private readonly ConfigRepo configRepo;

        public StrikeModule(StrikeRepo strikeRepo, ConfigRepo configRepo)
        {
            this.configRepo = configRepo;
            this.strikeRepo = strikeRepo;
        }

        [Command("strike")]
        public async Task Strike(int amount, SocketUser user, [Remainder] string reason = null)
        {
            // reason ??= "No reason stated.";
            // var striker = Context.Message.Author;
            // var guildId = Context.Guild.Id;
            // var strikeInfo = await strikeRepo.GetUser(guildId, user.Id);
            // var oldStrikes = strikeInfo.Strikes;
            // var strikes = await strikeRepo.AddStrikes(guildId, user.Id, striker.Id, amount, reason);

            // var sb = new StringBuilder();
            // sb.AppendLine($"{Formatter.NowBlock()} {Emotes.RedFlag} {Formatter.FullName(striker)} gave `{amount}` strikes `[{oldStrikes} → {strikes}]` to {Formatter.FullName(user, true)}");
            // sb.AppendLine($"`[Reason]` {reason}");
            // await ReplyAsync(sb.ToString());
            await EditStrikes(amount, user, reason, Context, isPardon: false);
        }

        [Command("pardon")]
        public async Task Pardon(int amount, SocketUser user, [Remainder] string reason = null)
        {
            // reason ??= "No reason stated.";
            // var pardoner = Context.Message.Author;
            // var guildId = Context.Guild.Id;
            // var strikeInfo = await strikeRepo.GetUser(guildId, user.Id);
            // var oldStrikes = strikeInfo.Strikes;
            // var strikes = await strikeRepo.RemoveStrikes(guildId, user.Id, pardoner.Id, amount, reason);

            // var sb = new StringBuilder();
            // sb.AppendLine($"{Formatter.NowBlock()} {Emotes.WhiteFlag} {Formatter.FullName(pardoner)} pardoned `{amount}` strikes `[{oldStrikes} → {strikes}]` from {Formatter.FullName(user, true)}");
            // sb.AppendLine($"`[Reason]` {reason}");
            // await ReplyAsync(sb.ToString());
            await EditStrikes(amount, user, reason, Context, isPardon: true);
        }

        private async Task EditStrikes(int amount, SocketUser user, string reason, SocketCommandContext context, bool isPardon)
        {
            reason ??= "No reason stated.";
            var pardoner = context.Message.Author;
            var guildId = context.Guild.Id;
            var strikeInfo = await strikeRepo.GetUser(guildId, user.Id);
            var oldStrikes = strikeInfo.Strikes;
            int strikes;
            if (isPardon)
            {
                strikes = await strikeRepo.RemoveStrikes(guildId, user.Id, pardoner.Id, amount, reason);
            }
            else
            {
                strikes = await strikeRepo.AddStrikes(guildId, user.Id, pardoner.Id, amount, reason);
            }

            var sb = new StringBuilder();
            sb.AppendLine($"{Formatter.NowBlock()} {(isPardon ? Emotes.WhiteFlag : Emotes.RedFlag)} {Formatter.FullName(pardoner)} {(isPardon ? "pardoned" : "gave")} `{amount}` strikes `[{oldStrikes} → {strikes}]` {(isPardon ? "from" : "to")} {Formatter.FullName(user, true)}");
            sb.AppendLine($"`[Reason]` {reason}");
            await ReplyAsync(sb.ToString());
        }

        [Command("check")]
        public async Task Check(SocketUser user)
        {
            var guild = Context.Guild;
            var strikeInfo = await strikeRepo.GetUser(guild.Id, user.Id);
            var sb = new StringBuilder();
            sb.AppendLine($"{Emotes.Magnifying} Moderation information for **{user.Username}**#{user.Discriminator} (ID:{user.Id}):");
            sb.AppendLine($"{Emotes.RedFlag} Strikes: **{strikeInfo.Strikes}**");
            if (strikeInfo.StrikeMetadata.Any())
            {
                sb.AppendLine($"{Emotes.RedBook} Latest strike history:");
                var history = GetHistory(user, 5, "strike", strikeInfo.StrikeMetadata, guild);
                sb.AppendLine(history);
            }
            if (strikeInfo.PardonMetadata.Any())
            {
                sb.AppendLine($"{Emotes.GreenBook} Latest pardon history:");
                var history = GetHistory(user, 5, "pardon", strikeInfo.PardonMetadata, guild);
                sb.AppendLine(history);
            }
            if (!strikeInfo.StrikeMetadata.Any() && !strikeInfo.PardonMetadata.Any())
            {
                sb.AppendLine($"{Emotes.BlueBook} No strike history found");
            }
            await ReplyAsync(sb.ToString());
        }

        [Command("strikehistory")]
        public async Task StrikeHistory(SocketUser user, int amount = 5)
        {
            var guild = Context.Guild;
            var strikeInfo = await strikeRepo.GetUser(guild.Id, user.Id);
            var history = GetHistory(user, amount, "strike", strikeInfo.StrikeMetadata, guild);
            await ReplyAsync(history);
        }

        [Command("pardonhistory")]
        public async Task PardonHistory(SocketUser user, int amount = 5)
        {
            var guild = Context.Guild;
            var strikeInfo = await strikeRepo.GetUser(guild.Id, user.Id);
            var history = GetHistory(user, amount, "pardon", strikeInfo.PardonMetadata, guild);
            await ReplyAsync(history);
        }

        private string GetHistory(SocketUser user, int amount, string type, IEnumerable<HistoryMetadata> history, SocketGuild guild)
        {
            var sb = new StringBuilder();
            if (history.Any())
            {
                sb.AppendLine($"Last {amount} {type}(s) for {Formatter.FullName(user, true)}:");
                var lastPardons = history.OrderByDescending(m => m.Timestamp).Take(amount);
                foreach (var pardon in lastPardons)
                {
                    var pardoner = guild.GetUser(pardon.GivenBy);
                    sb.AppendLine(pardon.ToString(pardoner));
                }
            }
            else
            {
                sb.AppendLine($"No {type} history found for {Formatter.FullName(user, true)}");
            }
            return sb.ToString();
        }
    }
}