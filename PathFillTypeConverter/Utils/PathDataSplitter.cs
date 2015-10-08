using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFillTypeConverter.Utils
{
    public static class PathDataSplitter
    {
        private static bool IsCommand(char c)
        {
            return "MZLHVCSQTA".IndexOf(char.ToUpperInvariant(c)) >= 0;
        }

        private static bool IsDelimiter(char c)
        {
            return char.IsWhiteSpace(c) || c == ',';
        }

        private static IEnumerable<string> SplitByCommandsInternal(string s)
        {
            var buffer = new StringBuilder();
            foreach (var c in s)
            {
                if (IsCommand(c))
                {
                    yield return buffer.ToString();
                    buffer.Clear();
                }
                buffer.Append(c);
            }
            yield return buffer.ToString();
        }

        private static IEnumerable<string> FilterEmpty(this IEnumerable<string> entries)
        {
            return entries.Select(x => x.Trim()).Where(x => x.Length > 0);
        }

        public static IEnumerable<string> SplitByCommands(string s)
        {
            return SplitByCommandsInternal(s).FilterEmpty();
        }

        private static IEnumerable<string> SplitParametersInternal(string s)
        {
            var buffer = new StringBuilder();
            Func<bool> bufferEndsWithE = () => buffer.ToString().EndsWith("E", StringComparison.InvariantCultureIgnoreCase);
            Func<bool> bufferContainsDot = () => buffer.ToString().Contains(".");
            foreach (var c in s)
            {
                if (IsDelimiter(c) ||
                    ((c == '+' || c == '-') && !bufferEndsWithE()) ||
                    (c == '.' && bufferContainsDot()))
                {
                    yield return buffer.ToString();
                    buffer.Clear();
                }
                if (!IsDelimiter(c))
                {
                    buffer.Append(c);
                }
            }
            yield return buffer.ToString();
        }

        public static IEnumerable<string> SplitParameters(string s)
        {
            return SplitParametersInternal(s).FilterEmpty();
        }
    }
}
