using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentResults;
using SshTools.Config.Parameters;
using SshTools.Config.Parser;
using SshTools.Config.Util;

namespace SshTools.Config.Parents
{
    public class SshConfig : ParameterParent
    {
        public string FileName { get; private set; }
        private readonly IList<string> _footer = new List<string>();

        /// <summary>
        /// Create a new SshConfig
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="parameters"></param>
        /// <exception cref="ArgumentException">Throws argument exception, if there is no HOST/MATCH keyword defined</exception>
        public SshConfig(string fileName = null, IList<IParameter> parameters = null)
            : base(parameters)
        {
            FileName = fileName;
        }
        
        public new string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT)
        {
            var lines = new List<string>();
            lines.AddRange(this.Select(p => p.Serialize(options)));
            lines.AddRange(_footer);
            return string.Join(Environment.NewLine, lines);
        }
        
        /// Parses a file by path.
        /// <param name="path">Path to file</param>
        /// <returns>SshConfig</returns>
        public static Result<SshConfig> ReadFile(string path)
        {
            var readRes = Result.Try(() => File.ReadAllText(path));
            return readRes.IsFailed
                ? readRes.ToResult<SshConfig>()
                : DeserializeString(readRes.Value, path);
        }

        /// Parses the SSH config text.
        /// <param name="str">Config string</param>
        /// <param name="fileName"></param>
        /// <returns>SshConfig</returns>
        public static Result<SshConfig> DeserializeString(string str, string fileName = null)
        {
            var config = new SshConfig(fileName);
            var res = config.Deserialize(str);
            return res.IsSuccess
                ? Result.Ok(config)
                : res.ToResult<SshConfig>();
        }

        private Result Deserialize(string configString)
        {
            var comments = new CommentList();
            ParameterParent lastNode = this;
            foreach (var l in configString.Split('\n'))
            {
                var line = l.Replace("\r", "");
                // Go for all comments (empty lines and comments, that are being stripped of their first #)
                if (LineParser.IsConfigComment(line))
                {
                    var comment = LineParser.TrimFront(line, out var spacingComment);

                    comments.Add(comment.StartsWith("#") 
                            ? comment.Substring(1, comment.Length - 1) 
                            : comment, 
                        spacingComment);
                    continue;
                }
                line = LineParser.TrimFront(line, out var spacingFront);
                
                line = LineParser.TrimKey(line, out var keyRes);
                if (keyRes.IsFailed) return keyRes.ToResult();

                var keyString = keyRes.Value;
                if (!SshTools.Settings.HasKeyword(keyString))
                    return Result.Fail($"Unknown Keyword {keyRes.Value}");
                var key = SshTools.Settings.GetKeyword(keyString.ToUpper());
                
                line = LineParser.TrimSeparator(line, out var separatorRes);
                if (separatorRes.IsFailed) return separatorRes.ToResult();

                var spacingBack = LineParser.TrimArgument(line, out var argumentRes, out var quoted);

                var appearance = new ParameterAppearance(
                    comments.ToCommentList(),
                    spacingFront,
                    keyString,
                    separatorRes.Value,
                    quoted,
                    spacingBack
                );
                var paramRes = key.GetParameter(argumentRes, appearance);
                if (paramRes.IsFailed) return paramRes.ToResult();
                var param = paramRes.Value;
                if (param.Argument is Node node)
                {
                    lastNode = node;
                    Add(param);
                }
                else
                {
                    lastNode.Add(param);
                }
                comments.Clear();
            }
            foreach (var comment in comments) _footer.Add(comment);
            return Result.Ok();
        }
        
        public override object Clone()
        {
            var parent = new SshConfig();
            foreach (var parameter in this) 
                parent.Add(parameter.Clone());
            return parent;
        }

        public HostNode this[string hostName] => this.Get(hostName);
    }
}