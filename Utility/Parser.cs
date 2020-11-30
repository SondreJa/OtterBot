using System;
using System.Text.RegularExpressions;

namespace OtterBot.Utility
{
    public class Parser
    {
        private const string SpanPattern = @"(\d+)([wdhms])";

        public static bool TryParseToSpan(string length, out TimeSpan? span)
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

        private static TimeSpan SwitchOnInterval(int length, string interval) =>
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