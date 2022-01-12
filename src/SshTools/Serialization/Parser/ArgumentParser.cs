using FluentResults;
using SshTools.Parent;
using SshTools.Parent.Host;
using SshTools.Parent.Match;

namespace SshTools.Serialization.Parser
{
    public static class ArgumentParser
    {
        public static readonly ArgumentParser<HostNode> Host = new ArgumentParser<HostNode>(
            HostNode.Of,
            (value, options) => value.Serialize(options)
        );
        
        public static readonly ArgumentParser<MatchNode> Match = new ArgumentParser<MatchNode>(
            MatchNode.Of,
            (value, options) => value.Serialize(options)
        );
        
        public static readonly ArgumentParser<SshConfig> SshConfig = new ArgumentParser<SshConfig>(
            Parent.SshConfig.ReadFile,
            (value, options) => value.FileName ?? value.Serialize(options)
        );
        
        public static readonly ArgumentParser<string> String = new ArgumentParser<string>(
            Result.Ok, 
            (value, options) => value
            );
        
        public static readonly ArgumentParser<ushort> UShort = new ArgumentParser<ushort>(ushort.TryParse);
        
        public static readonly ArgumentParser<bool> YesNo = new ArgumentParser<bool>(
            str =>
            {
                switch (str.ToLower())
                {
                    case "yes":
                        return Result.Ok(true);
                    case "no":
                        return Result.Ok(false);
                    default:
                        return Result.Fail<bool>($"Could not parse argument. Got invalid invalid input string '{str}'!");
                }
            }, 
            (value, options) => value ? "yes" : "no",
            new []{ "no", "yes" });
    }
}