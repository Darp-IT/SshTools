using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentResults;
using SshTools.Config.Matching;

namespace SshTools.Config.Parser
{
    public static class MatchStringParser
    {
        private static readonly Regex Regex = new Regex("[^\\s]+|\\s+", RegexOptions.Compiled);
        
        public static Result<IList<(Criteria, string, string, string)>> Parse(string str)
        {
            var matches = Regex.Matches(str);
            IList<(Criteria, string, string, string)> list = new List<(Criteria, string, string, string)>();

            var i = 0;
            while (i < matches.Count)
            {
                var match = matches[i];
                var val = match.Value;
                if (!SshTools.Settings.HasCriteria(val))
                    return Result.Fail($"Expected a criteria! '{val}' is not matching at position {i} of string '{str}'");
                
                var criteria = SshTools.Settings.GetCriteria(val);
                string spacing = null;
                string value = null;
                
                if (criteria is ArgumentCriteria)
                {
                    if (i + 2 >= matches.Count)
                        return Result.Fail($"Got argument criteria '{val}' at position {i} of string '{str}'," + 
                                           " but could not find an argument afterwards");
                    spacing = matches[i + 1].Value;
                    value = matches[i + 2].Value;
                    i += 2;
                }
                string spacingBack = null;
                if (matches.Count > i + 1)
                {
                    spacingBack = matches[i + 1].Value;
                    i++;
                }
                list.Add((criteria, spacing, value, spacingBack));
                i++;
            }
            return Result.Ok(list);
        }
    }
}