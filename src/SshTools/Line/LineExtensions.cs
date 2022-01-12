using SshTools.Line.Parameter;
using SshTools.Line.Parameter.Keyword;
using SshTools.Util;

namespace SshTools.Line
{
    public static class LineExtensions
    {
        public static bool Is(this ILine line, Keyword keyword)
        {
            line.ThrowIfNull();
            keyword.ThrowIfNull();
            if (!(line is IParameter parameter)) return false;
            return keyword.Equals(parameter.Keyword);
        }

        public static bool Is<T>(this ILine line, bool includeSubTypes = false)
        {
            if (!(line is IParameter parameter)) return false;
            return parameter.Keyword.Is<T>(includeSubTypes);
        }

        public static bool IsHost(this ILine line)
        {
            if (!(line is IParameter parameter)) return false;
            return parameter.Keyword.IsHost();
        }

        public static bool IsMatch(this ILine line)
        {      
            if (!(line is IParameter parameter)) return false;
            return parameter.Keyword.IsMatch();
        }

        public static bool IsNode(this ILine line)
        {
            if (!(line is IParameter parameter)) return false;
            return parameter.Keyword.IsNode();
        }

        public static bool IsInclude(this ILine line)
        {
            if (!(line is IParameter parameter)) return false;
            return parameter.Keyword.IsInclude();
        }
    }
}