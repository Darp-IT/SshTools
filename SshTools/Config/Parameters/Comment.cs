using System;
using SshTools.Config.Util;

namespace SshTools.Config.Parameters
{
    public class Comment : ILine
    {
        public string Spacing { get; set; }
        public string Argument { get; }

        public Comment(string comment, string spacing = "")
        {
            Argument = comment;
            Spacing = spacing;
        }
        
        public string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT) => GenerateComment();
        public string GenerateComment() => Argument.Trim().Length > 0
            ? Spacing + "#" + Argument
            : Spacing;
        public override string ToString() => $"Comment={GenerateComment()}";
        public ILine Clone() => new Comment(Argument, Spacing);
        object ICloneable.Clone() => Clone();
    }
}