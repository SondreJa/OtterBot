using System;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using OtterBot.Models;
using OtterBot.Utility;

namespace OtterBot.Handlers
{
    public class StrikeHandler
    {
        public async Task<string> HandleStrike(SocketUser user, StrikeAction action)
        {
            switch (action.Action)
            {
                case BotAction.Mute:
                    return $"Muting {Formatter.FullName(user, true)}{(action.Length.HasValue ? $" for{TimespanToString(action.Length.Value)}" : string.Empty)}.";
                case BotAction.Kick:
                    return $"Kicking {Formatter.FullName(user, true)}.";
                case BotAction.Ban:
                    return $"Banning {Formatter.FullName(user, true)}{(action.Length.HasValue ? $" for{TimespanToString(action.Length.Value)}" : string.Empty)}.";
                case BotAction.Nothing:
                default:
                    return string.Empty;
            }
        }

        private string TimespanToString(TimeSpan timeSpan)
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