using SshTools.Parent;
using SshTools.Parent.Host;
using SshTools.Parent.Match;
using SshTools.Serialization.Parser;

namespace SshTools.Line.Parameter.Keyword
{
    public partial class Keyword
    {
        public static readonly Keyword<HostNode> Host = 
            new Keyword<HostNode>(nameof(Host), ArgumentParser.Host, allowMultiple:true);
        public static readonly Keyword<MatchNode> Match = 
            new Keyword<MatchNode>(nameof(Match), ArgumentParser.Match, allowMultiple:true);
        public static readonly Keyword<SshConfig> Include = 
            new Keyword<SshConfig>(nameof(Include), ArgumentParser.SshConfig, allowMultiple:true);

        public static readonly Keyword<string> User = 
            new Keyword<string>(nameof(User), ArgumentParser.String);
        public static readonly Keyword<string> HostName = 
            new Keyword<string>(nameof(HostName), ArgumentParser.String);
        public static readonly Keyword<string> ProxyJump = 
            new Keyword<string>(nameof(ProxyJump), ArgumentParser.String);
        public static readonly Keyword<string> IdentityFile = 
            new Keyword<string>(nameof(IdentityFile), ArgumentParser.String, allowMultiple:true);
        public static readonly Keyword<ushort> Port = 
            new Keyword<ushort>(nameof(Port), ArgumentParser.UShort);
        public static readonly Keyword<bool> IdentitiesOnly = 
            new Keyword<bool>(nameof(IdentitiesOnly), ArgumentParser.YesNo);
    }
}