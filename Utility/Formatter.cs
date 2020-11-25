using System;
using System.Text;
using Discord.WebSocket;

namespace OtterBot.Utility
{
    public class Formatter
    {
        public static string FullName(SocketUser user, bool withId = false)
        {
            var sb = new StringBuilder();
            sb.Append($"**{user.Username}**#{user.Discriminator}");
            if (withId)
            {
                sb.Append($" (ID: {user.Id})");
            }
            return sb.ToString();
        }

        public static string NowBlock() => $"`[{DateTime.UtcNow.ToString("HH:mm:ss")}]`";
    }
}