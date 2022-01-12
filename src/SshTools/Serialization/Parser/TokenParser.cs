using System;
using System.Text.RegularExpressions;
using FluentResults;
using SshTools.Parent.Match;
using SshTools.Parent.Match.Token;

namespace SshTools.Serialization.Parser
{
    public static class TokenParser
    {
        private static readonly Regex GetPercentagesRegex = new Regex("%.", RegexOptions.Compiled);
        private static readonly Regex GetEnvVariablesRegex = new Regex("\\$\\{([^}]+)\\}", RegexOptions.Compiled);
        public static Result<string> Parse(string search, MatchingContext context)
        {
            return Result.Try(() =>
            {
                // Replace all environment variables
                search = GetEnvVariablesRegex.Replace(search, match =>
                    Environment.GetEnvironmentVariable(match.Groups[1].Value));
                // Replace all tildes to the home dir
                search = search.Replace("~",
                    Environment.OSVersion.Platform == PlatformID.Unix || 
                    Environment.OSVersion.Platform == PlatformID.MacOSX
                        ? Environment.GetEnvironmentVariable("HOME")
                        : Environment.GetEnvironmentVariable("UserProfile"));
                // Replace percents
                search = GetPercentagesRegex.Replace(search, match =>
                {
                    var tokenChar = match.Value[1];
                    if (!SshTools.Settings.Has<Token>(tokenChar))
                        throw new Exception($"Could not replace tokens - unknown token {tokenChar}");
                    return SshTools.Settings.Get<Token>(tokenChar).Apply(context);
                });
                return search;
            });
        }
    }
}