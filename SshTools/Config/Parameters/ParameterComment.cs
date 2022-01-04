namespace SshTools.Config.Parameters
{
    public struct ParameterComment
    {
        public ParameterComment(string comment) : this(comment, "")
        {
            
        }
        
        public ParameterComment(string comment, string spacing)
        {
            Comment = comment;
            Spacing = spacing;
        }

        public string Spacing { get; set; }
        public string Comment { get; set; }

        public string GenerateComment() => Comment.Trim().Length > 0
            ? Spacing + "#" + Comment
            : Spacing;

        public override string ToString() => GenerateComment();
    }
}