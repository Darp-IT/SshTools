using System;
using System.Text.RegularExpressions;
using FluentResults;
using SshTools.Config.Matching;

namespace SshTools.Config.Parser
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
                    if (!SshTools.Settings.HasToken(tokenChar))
                        throw new Exception($"Could not replace tokens - unknown token {tokenChar}");
                    return SshTools.Settings.GetToken(tokenChar).Apply(context);
                });
                return search;
            });
        }
        
        /*public static Result<string> Parse(string search, MatchingContext context)
        {
            var pointer = 0;
            while (search.Length > pointer + 1)
            {
                if (search[pointer] is '%' && !(search[pointer + 1] is '%'))
                {
                    var tokenChar = search[pointer + 1];
                    if (!SshTools.Settings.HasToken(tokenChar))
                        return Result.Fail<string>($"Could not replace tokens - unknown token {tokenChar}");
                    var res = Result.Try(() => SshTools.Settings.GetToken(tokenChar).Apply(context));
                    if (res.IsFailed)
                        return res.ToResult<string>();
                    search = search.Remove(pointer, 2);
                    search = search.Insert(pointer, res.Value);
                }
                else
                {
                    pointer++;
                }
            }
            return Result.Ok(search);
        }*/
    }
}