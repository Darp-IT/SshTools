using System.Text.RegularExpressions;
using FluentResults;

namespace SshTools.Config.Parser
{
    public static class LineParser
    {
        private static readonly Regex IsLineCommentRegex = new Regex("^(?!\\s*[a-zA-Z])", RegexOptions.Compiled);
        private static readonly Regex StartsWithLetterRegex = new Regex("^\\s");
        private static readonly Regex TrimFrontRegex = new Regex("^\\s*", RegexOptions.Compiled);
        private static readonly Regex TrimKeyRegex = new Regex("^[0-9a-zA-Z]*", RegexOptions.Compiled);
        private static readonly Regex TrimSeparatorRegex = new Regex("^[\t =\v]+", RegexOptions.Compiled);
        private static readonly Regex TrimSeparatorMultipleEqualsRegex = new Regex("(.*=.*){2,}", RegexOptions.Compiled);
        private static readonly Regex TrimQuotedArgumentRegex = new Regex("^\".*\"", RegexOptions.Compiled);
        private static readonly Regex TrimSpacingBack = new Regex("\\s+$", RegexOptions.Compiled);
        
        /// <summary>
        /// Returns whether the line is a commend (starts with #)
        /// </summary>
        /// <param name="line">line string</param>
        /// <returns>IsComment</returns>
        public static bool IsConfigComment(string line) => IsLineCommentRegex.IsMatch(line);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line">line string</param>
        /// <param name="spacingFront">optional out, might contain an error</param>
        /// <returns></returns>
        public static string TrimFront(string line, out string spacingFront)
        {
            // Trim the line, but save it for reconstruction
            var match = TrimFrontRegex.Match(line);
            spacingFront = match.Value;
            return TrimFrontRegex.Replace(line, "");
        }

        /// <summary>
        /// Trims the key from the line, expects no spaces at the beginning
        /// </summary>
        /// <param name="line">line string</param>
        /// <param name="key">optional out, might contain an error</param>
        /// <returns>trimmed string</returns>
        public static string TrimKey(string line, out Result<string> key)
        {
            if (StartsWithLetterRegex.IsMatch(line))
            {
                key = Result.Fail<string>($"Line does not start with a letter {line}");
                return line;
            }
            var match = TrimKeyRegex.Match(line);
            key = Result.Ok(match.Value);
            return TrimKeyRegex.Replace(line, "");
        }
        
        public static string TrimSeparator(string line, out Result<string> separator)
        {
            var match = TrimSeparatorRegex.Match(line);
            if (!match.Success)
            {
                separator = Result.Fail<string>($"Could not find a separator in line {line}");
                return line;
            }

            var sep = match.Value;
            if (TrimSeparatorMultipleEqualsRegex.IsMatch(sep))
            {
                separator = Result.Fail<string>($"Multiple '=' in separator {sep} of line {line}");
                return line;
            }
                
            separator = Result.Ok(sep);
            return TrimSeparatorRegex.Replace(line, "");
        }

        public static string TrimArgument(string line, out Result<string> argument, out bool quoted)
        {
            var match = TrimQuotedArgumentRegex.Match(line);
            quoted = match.Success;
            if (quoted)
            {
                var arg = match.Value;
                argument = Result.Ok(arg.Substring(1, arg.Length - 2));
                return TrimQuotedArgumentRegex.Replace(line, "");
            }
            argument = Result.Ok(TrimSpacingBack.Replace(line, ""));
            return TrimSpacingBack.Match(line).Value;
        }
    }
}