using SshTools.Line.Comment;

namespace SshTools.Line.Parameter
{
    public interface IParameter : ILine
    {
        Keyword.Keyword Keyword { get; }
        object Argument { get; set; }
        CommentList Comments { get; }
    }
}