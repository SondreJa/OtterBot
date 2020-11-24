using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System;
using Discord;

namespace OtterBot.Commands
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public Task Ping() => ReplyAsync("Pong");

        [Command("echo")]
        [Summary("Echoes a message")]
        public Task Echo([Remainder][Summary("The text to echo")] string echo) => ReplyAsync(echo);

        [Command("userinfo")]
        [Summary("Returns info about the user, or yourself if no user provided.")]
        [Alias("user", "whois")]
        public async Task UserInfo([Summary("The (optional) user to get info from")] SocketUser user = null)
        {
            if (user == null)
            {
                user = Context.Message.Author;
            }
            await ReplyAsync(FormatUserinfo(user));
        }

        [Command("serverinfo")]
        [Summary("Returns info about the server")]
        public async Task ServerInfo()
        {
            await ReplyAsync(FormatServerInfo(Context.Guild));
        }

        [Command("avatar")]
        [Summary("Returns the user's avatar")]
        [Alias("picture", "image", "pfp")]
        public async Task Avatar([Summary("The user to get the avatar from")] SocketUser user = null)
        {
            if (user == null)
            {
                await ReplyAsync("Please provide a user.");
                return;
            }
            var embedBuilder = new EmbedBuilder();
            embedBuilder.ImageUrl = user.GetAvatarUrl();
            await ReplyAsync(embed: embedBuilder.Build());

        }

        private string FormatUserinfo(SocketUser user)
        {
            var sb = new StringBuilder();
            var guildUser = Context.Guild.GetUser(user.Id);
            sb.AppendLine("```cs");
            sb.AppendLine($"{"User",10}: {user.Username}#{user.Discriminator}");
            sb.AppendLine($"{"ID",10}: {user.Id}");
            sb.AppendLine($"{"Created",10}: {GetHowLongAgo(user.CreatedAt)} ({user.CreatedAt})");
            sb.AppendLine($"{"Joined",10}: {GetHowLongAgo(guildUser.JoinedAt ?? new())} ({guildUser.JoinedAt})");
            sb.AppendLine($"{"Roles",10}: {string.Join(", ", guildUser.Roles.Select(r => r.ToString()))}");
            sb.AppendLine("```");
            return sb.ToString();
        }

        private string FormatServerInfo(SocketGuild guild)
        {
            var sb = new StringBuilder();
            sb.AppendLine("```cs");
            sb.AppendLine($"{"Server",10}: {guild.Name}");
            sb.AppendLine($"{"ID",10}: {guild.Id}");
            sb.AppendLine($"{"Members",10}: {guild.MemberCount} ({guild.Users.Where(u => u.Status != UserStatus.Offline && u.Status != UserStatus.Invisible).Count()} online)");
            sb.AppendLine($"{"Icon",10}: {guild.IconUrl}");
            sb.AppendLine($"{"Roles",10}: {string.Join(", ", guild.Roles.Select(r => r.ToString()))}");
            sb.AppendLine("```");
            return sb.ToString();
        }

        private string GetHowLongAgo(DateTimeOffset dts) =>
            (DateTimeOffset.UtcNow - dts.ToUniversalTime()) switch
            {
                // Lazy month and year handling
                { TotalHours: < 1 } ts => $"{ts.Minutes} minutes ago",
                { TotalDays: < 1 } ts => $"{ts.Hours} hours ago",
                { TotalDays: < 4 } ts => $"{ts.Days / 7} weeks ago",
                { TotalDays: < 365 } ts => $"{ts.Days / 30} months ago",
                var ts => $"{ts.Days / 365} year(s) ago"
            };
    }
}