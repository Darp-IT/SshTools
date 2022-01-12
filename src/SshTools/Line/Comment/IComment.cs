using SshTools.Serialization;

namespace SshTools.Line.Comment
{
    public interface IComment : ILine
    {
        string Argument { get; }
        string GenerateComment(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT);
    }
}