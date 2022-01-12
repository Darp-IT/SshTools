using System.Linq;
using System.Text.RegularExpressions;

namespace SshTools.Serialization.Parser
{
    // Based on code from https://github.com/dotnil/ssh-config
    public static class Globber
    {
        private static readonly Regex PatternSplitter =  new Regex("[,\\s]+");
        
        private static bool Match(string pattern, string str)
        {
            pattern = pattern.Replace(".", "\\.")
                .Replace("*", ".*")
                .Replace("?", ".{1}");

            return new Regex("^(?:" + pattern + ")$").IsMatch(str);
        }

        /// <summary>
        /// A helper function to match input against
        /// <a href="http://man.openbsd.org/OpenBSD-current/man5/ssh_config.5#PATTERNS">[pattern-list]</a>.
        /// According to `man ssh_config`, negated patterns shall be matched first.
        /// </summary>
        /// <param name="patternList">The string, that contains the pattern (with * or !)</param>
        /// <param name="str">The string that should be checked for</param>
        /// <returns>Whether <see cref="str"/> matches the <see cref="patternList"/></returns>
        public static bool Glob(string patternList, string str)
        {
            if (string.IsNullOrEmpty(str))
                return true;
            var patterns = PatternSplitter.Split(patternList)
                .OrderByDescending(a => a.StartsWith("!"));

            foreach (var pattern in patterns)
            {
                var negate = pattern[0] == '!';
                if (negate && Match(pattern.Substring(1), str))
                {
                    return false;
                }

                if (Match(pattern, str))
                {
                    return true;
                }
            }

            return false;
        }
    }
}