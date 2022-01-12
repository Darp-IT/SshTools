using SshTools.Serialization;

namespace SshTools.Line.Comment
{
    public class Comment : IComment
    {
        private readonly string _spacing;
        public string Argument { get; }

        internal Comment(string comment, string spacing = "")
        {
            Argument = comment;
            _spacing = spacing;
        }
        
        public string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT) => GenerateComment();
        public string GenerateComment(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT)
        {
            var spacing = options.HasFlag(SerializeConfigOptions.TRIM_FRONT) ? "" : _spacing;
            return string.IsNullOrEmpty(Argument)
                ? spacing
                : spacing + "#" + Argument;
        }

        public override string ToString() => $"Comment={GenerateComment()}";
        public object Clone() => new Comment(Argument, _spacing);
    }
}