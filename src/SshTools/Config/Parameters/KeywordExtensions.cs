using SshTools.Config.Extensions;
using SshTools.Config.Parents;

namespace SshTools.Config.Parameters
{
    public static class KeywordExtensions
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

        public static bool IsHost(this Keyword keyword) => keyword.Is<HostNode>();
        public static bool IsMatch(this Keyword keyword) => keyword.Is<MatchNode>();
        public static bool IsNode(this Keyword keyword) => keyword.Is<Node>(true);
        public static bool IsInclude(this Keyword keyword) => keyword.Is<SshConfig>();
    }
}