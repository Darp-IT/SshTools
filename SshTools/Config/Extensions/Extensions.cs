using System;
using System.Runtime.CompilerServices;

namespace SshTools.Config.Extensions
{
    internal static class Extensions
    {
        public static void ThrowIfNull(this object obj, [CallerMemberName] string name = null)
        {
            if (obj == null)
                throw new ArgumentNullException(name);
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            var pos = text.IndexOf(search, StringComparison.Ordinal);
            return pos < 0
                ? text
                : text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}