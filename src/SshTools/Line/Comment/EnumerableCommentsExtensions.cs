using System.Collections.Generic;

namespace SshTools.Line.Comment
{
    public static class EnumerableCommentsExtensions
    {
        public static CommentList ToCommentList(this IEnumerable<string> list)
        {
            var res = new CommentList();
            if (list is CommentList commentList)
                res.AddRange(commentList.Comments);
            else
                foreach (var l in list) res.Add(l);
            return res;
        }
        
        public static CommentList ToCommentList(this IEnumerable<IComment> list)
        {
            var res = new CommentList();
            res.AddRange(list);
            return res;
        }
    }
}