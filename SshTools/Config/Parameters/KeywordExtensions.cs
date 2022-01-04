using SshTools.Config.Extensions;
using SshTools.Config.Parents;

namespace SshTools.Config.Parameters
{
    public static class KeywordExtensions
    {
        public static bool Is(this IParameter parameter, Keyword keyword)
        {
            parameter.ThrowIfNull();
            keyword.ThrowIfNull();
            return keyword.Equals(parameter.Keyword);
        }

        public static bool Is<T>(this IParameter parameter, bool includeSubTypes = false) =>
            parameter.Keyword.Is<T>(includeSubTypes);
        public static bool IsHost(this IParameter parameter) => parameter.Keyword.IsHost();
        public static bool IsMatch(this IParameter parameter) => parameter.Keyword.IsMatch();
        public static bool IsNode(this IParameter parameter) => parameter.Keyword.IsNode();
        public static bool IsInclude(this IParameter parameter) => parameter.Keyword.IsInclude();
        
        public static bool IsHost(this Keyword keyword) => keyword.Is<HostNode>();
        public static bool IsMatch(this Keyword keyword) => keyword.Is<MatchNode>();
        public static bool IsNode(this Keyword keyword) => keyword.Is<Node>(true);
        public static bool IsInclude(this Keyword keyword) => keyword.Is<SshConfig>();
    }
}