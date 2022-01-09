using System;
using System.Collections.Generic;
using System.Linq;
using SshTools.Config.Exceptions;
using SshTools.Config.Extensions;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Parser;

namespace SshTools.Config.Parents
{
    public static class ParentExtensions
    {
        /// <summary>
        /// Deserializes a string into a sequence of lines
        /// </summary>
        /// <param name="configString">The given string, that represents a ssh config</param>
        /// <returns>A sequence of lines representing <paramref name="configString"/></returns>
        /// <exception cref="ResultException">Thrown if something goes wrong while parsing</exception>
        /// <exception cref="Exception">Thrown if something goes wrong while parsing</exception>
        internal static IEnumerable<ILine> Deserialized(this string configString)
        {
            foreach (var l in configString.Split('\n'))
            {
                var line = l.Replace("\r", "");
                // Go for all comments (empty lines and comments, that are being stripped of their first #)
                if (LineParser.IsConfigComment(line))
                {
                    var comment = LineParser.TrimFront(line, out var spacingComment);

                    yield return new Comment(
                        comment.StartsWith("#")
                            ? comment.Substring(1, comment.Length - 1)
                            : comment,
                        spacingComment);
                    continue;
                }

                line = LineParser.TrimFront(line, out var spacingFront);

                line = LineParser.TrimKey(line, out var keyRes);
                if (keyRes.IsFailed)
                    throw new ResultException(keyRes.WithError($"While parsing line '{l}'"));

                var keyString = keyRes.Value;
                if (!SshTools.Settings.HasKeyword(keyString))
                    throw new Exception($"Unknown Keyword {keyRes.Value} while parsing line '{l}'");

                var key = SshTools.Settings.GetKeyword(keyString);

                line = LineParser.TrimSeparator(line, out var separatorRes);
                if (separatorRes.IsFailed)
                    throw new ResultException(separatorRes.WithError($"While parsing line '{l}'"));

                var spacingBack = LineParser.TrimArgument(line, out var argumentRes, out var quoted);

                var appearance = new ParameterAppearance(
                    spacingFront,
                    keyString,
                    separatorRes.Value,
                    quoted,
                    spacingBack
                );
                var paramRes = key.GetParameter(argumentRes, appearance);
                if (paramRes.IsFailed)
                    throw new ResultException(paramRes.WithError($"While parsing line '{l}'"));
                yield return paramRes.Value;
            }
        }
        
        /// <summary>
        /// Creates a new SshConfig, with all includes
        /// </summary>
        /// <returns>New SshConfig</returns>
        public static SshConfig Compile(this SshConfig sshConfig) => sshConfig
            .Compiled()
            .ToConfig();
        
        public static IList<HostNode> Hosts(this SshConfig sshConfig) => sshConfig
            .WhereArg<HostNode>()
            .SelectArg()
            .ToList();
        
        public static IList<Node> Nodes(this SshConfig sshConfig) => sshConfig
            .WhereArg<Node>()
            .SelectArg()
            .ToList();

        public static IList<MatchNode> Matches(this SshConfig sshConfig) => sshConfig
            .WhereArg<MatchNode>()
            .SelectArg()
            .ToList();
        
        public static TP PushHost<TP>(this TP parent, string hostName, Action<HostNode> func = null)
            where TP : SshConfig
        {
            var res = parent.InsertHost(0, hostName);
            if (res.IsSuccess) func?.Invoke(res.Value);
            return parent;
        }
        
        public static TP PushMatch<TP>(this TP parent, Criteria criteria, Action<MatchNode> func = null)
            where TP : SshConfig
        {
            var res = parent.InsertMatch(0, criteria);
            if (res.IsSuccess) func?.Invoke(res.Value);
            return parent;
        }
        
        public static TP PushMatch<TP>(this TP parent, ArgumentCriteria criteria, string argument, Action<MatchNode> func = null)
            where TP : SshConfig
        {
            var res = parent.InsertMatch(0, criteria, argument);
            if (res.IsSuccess) func?.Invoke(res.Value);
            return parent;
        }
    }
}