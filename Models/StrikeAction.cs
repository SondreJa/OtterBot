using System;

namespace OtterBot.Models
{
    public class StrikeAction
    {
        public BotAction Action { get; set; }
        public TimeSpan? Length { get; set; }
    }
}