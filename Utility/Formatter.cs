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

        public static string TimespanToString(TimeSpan timeSpan)
        {
            var sb = new StringBuilder();
            if (timeSpan.Days > 0)
            {
                sb.Append($" {timeSpan.Days} days");
            }
            if (timeSpan.Hours > 0)
            {
                sb.Append($" {timeSpan.Hours} hours");
            }
            if (timeSpan.Minutes > 0)
            {
                sb.Append($" {timeSpan.Minutes} minutes");
            }
            if (timeSpan.Seconds > 0)
            {
                sb.Append($" {timeSpan.Seconds} seconds");
            }
            return sb.ToString();
        }
    }
}