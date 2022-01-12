using SshTools.Parent;
using SshTools.Parent.Host;
using SshTools.Parent.Match;

namespace SshTools.Line.Parameter.Keyword
{
    public static class KeywordExtensions
    {
        public static bool IsHost(this Keyword keyword) => keyword.Is<HostNode>();
        public static bool IsMatch(this Keyword keyword) => keyword.Is<MatchNode>();
        public static bool IsNode(this Keyword keyword) => keyword.Is<Node>(true);
        public static bool IsInclude(this Keyword keyword) => keyword.Is<SshConfig>();
    }
}