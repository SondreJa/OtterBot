using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using OtterBot.Constants;
using OtterBot.Repository;

namespace OtterBot.Commands
{
    public class StrikeModule : ModuleBase<SocketCommandContext>
    {

        private readonly StrikeRepo strikeRepo;
        public StrikeModule(StrikeRepo strikeRepo)
        {
            this.strikeRepo = strikeRepo;
        }

        [Command("strike")]
        public async Task Strike(int amount, SocketUser user, [Remainder] string reason = null)
        {
            reason ??= "No reason stated.";
            var striker = Context.Message.Author;
            var guildId = Context.Guild.Id;
            var strikeInfo = await strikeRepo.GetUser(guildId, user.Id);
            var oldStrikes = strikeInfo.Strikes;
            var strikes = await strikeRepo.AddStrikes(guildId, user.Id, striker.Id, amount, reason);

            var sb = new StringBuilder();
            sb.AppendLine($"`[{DateTime.UtcNow.ToString("HH:mm:ss")}]` {Emotes.RedFlag} **{striker.Username}**#{striker.Discriminator} gave `{amount}` strikes `[{oldStrikes} → {strikes}]` to **{user.Username}**#{user.Discriminator} (ID:{user.Id})");
            sb.AppendLine($"`[Reason]` {reason}");
            await ReplyAsync(sb.ToString());
        }

        [Command("pardon")]
        public async Task Pardon(int amount, SocketUser user, [Remainder] string reason = null)
        {
            reason ??= "No reason stated.";
            var pardoner = Context.Message.Author;
            var guildId = Context.Guild.Id;
            var strikeInfo = await strikeRepo.GetUser(guildId, user.Id);
            var oldStrikes = strikeInfo.Strikes;
            var strikes = await strikeRepo.RemoveStrikes(guildId, user.Id, pardoner.Id, amount, reason);

            var sb = new StringBuilder();
            sb.AppendLine($"`[{DateTime.UtcNow.ToString("HH:mm:ss")}]` {Emotes.WhiteFlag} **{pardoner.Username}**#{pardoner.Discriminator} pardoned `{amount}` strikes `[{oldStrikes} → {strikes}]` from **{user.Username}**#{user.Discriminator} (ID:{user.Id})");
            sb.AppendLine($"`[Reason]` {reason}");
            await ReplyAsync(sb.ToString());
        }

        [Command("check")]
        public async Task Check(SocketUser user)
        {
            var strikeInfo = await strikeRepo.GetUser(Context.Guild.Id, user.Id);
            var sb = new StringBuilder();
            sb.AppendLine($"{Emotes.Magnifying} Moderation information for **{user.Username}**#{user.Discriminator} (ID:{user.Id}):");
            sb.AppendLine($"{Emotes.RedFlag} Strikes: **{strikeInfo.Strikes}**");
            if (strikeInfo.StrikeMetadata.Any())
            {
                sb.AppendLine($"{Emotes.RedBook} Latest strike history:");
                var lastFive = strikeInfo.StrikeMetadata.OrderByDescending(m => m.Timestamp).Take(5);
                foreach (var strike in lastFive)
                {
                    var striker = Context.Guild.GetUser(strike.GivenBy);
                    sb.AppendLine($"`[{strike.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}]` {strike.Amount} strike(s) given by **{striker.Username}**#{striker.Discriminator} because: {strike.Reason}");
                }
            }
            if (strikeInfo.PardonMetadata.Any())
            {
                sb.AppendLine($"{Emotes.GreenBook} Latest pardon history:");
                var lastFive = strikeInfo.PardonMetadata.OrderByDescending(m => m.Timestamp).Take(5);
                foreach (var pardon in lastFive)
                {
                    var pardoner = Context.Guild.GetUser(pardon.GivenBy);
                    sb.AppendLine($"`[{pardon.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}]` {pardon.Amount} strike(s) pardoned by **{pardoner.Username}**#{pardoner.Discriminator} because: {pardon.Reason}");
                }
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
            var strikeInfo = await strikeRepo.GetUser(Context.Guild.Id, user.Id);
            var sb = new StringBuilder();
            if (strikeInfo.StrikeMetadata.Any())
            {
                sb.AppendLine($"Last {amount} strikes for **{user.Username}**#{user.Discriminator} (ID:{user.Id}):");
                var lastStrikes = strikeInfo.StrikeMetadata.OrderByDescending(m => m.Timestamp).Take(amount);
                foreach (var strike in lastStrikes)
                {
                    var striker = Context.Guild.GetUser(strike.GivenBy);
                    sb.AppendLine($"`[{strike.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}]` {strike.Amount} strike(s) given by **{striker.Username}**#{striker.Discriminator} because: {strike.Reason}");
                }
            }
            else
            {
                sb.AppendLine($"No strike history found for **{user.Username}**#{user.Discriminator} (ID:{user.Id})");
            }
            await ReplyAsync(sb.ToString());
        }

        [Command("pardonhistory")]
        public async Task PardonHistory(SocketUser user, int amount = 5)
        {
            var strikeInfo = await strikeRepo.GetUser(Context.Guild.Id, user.Id);
            var sb = new StringBuilder();
            if (strikeInfo.PardonMetadata.Any())
            {
                sb.AppendLine($"Last {amount} strikes for **{user.Username}**#{user.Discriminator} (ID:{user.Id}):");
                var lastPardons = strikeInfo.PardonMetadata.OrderByDescending(m => m.Timestamp).Take(amount);
                foreach (var pardon in lastPardons)
                {
                    var pardoner = Context.Guild.GetUser(pardon.GivenBy);
                    sb.AppendLine($"`[{pardon.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}]` {pardon.Amount} strike(s) pardoned by **{pardoner.Username}**#{pardoner.Discriminator} because: {pardon.Reason}");
                }
            }
            else
            {
                sb.AppendLine($"No strike history found for **{user.Username}**#{user.Discriminator} (ID:{user.Id})");
            }
            await ReplyAsync(sb.ToString());
        }
    }
}