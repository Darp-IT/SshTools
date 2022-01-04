using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SshTools.Config.Parameters
{
    public class CommentList : IList<string>
    {
        internal IList<ParameterComment> Comments { get; } = new List<ParameterComment>();
        public IEnumerator<string> GetEnumerator() => Comments
            .Select(c => c.Comment).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(string item) => 
            Comments.Add(new ParameterComment(item));
        public void Add(string item, string spacing) => 
            Comments.Add(new ParameterComment(item, spacing));

        public void Clear() => Comments.Clear();

        public bool Contains(string item) => Comments
            .Select(c => c.Comment).Contains(item);

        public void CopyTo(string[] array, int arrayIndex) => Comments
            .CopyTo(array.Select(a => new ParameterComment(a)).ToArray(), arrayIndex);

        public bool Remove(string item)
        {
            for (var i = Comments.Count - 1; i >= 0; i--)
            {
                if (!Comments[i].Comment.Equals(item)) continue;
                Comments.RemoveAt(i);
                return true;
            }
            return false;
        }
        public int Count => Comments.Count;
        public bool IsReadOnly => Comments.IsReadOnly;
        public int IndexOf(string item)
        {
            for (var i = 0; i < Comments.Count; i++)
            {
                if (!Comments[i].Comment.Equals(item))
                    continue;
                return i;
            }
            return -1;
        }

        public void Insert(int index, string item) => 
            Comments.Insert(index, new ParameterComment(item));

        public void RemoveAt(int index) => Comments.RemoveAt(index);

        public string this[int index]
        {
            get => Comments[index].Comment;
            set => Comments[index] = new ParameterComment(value);
        }
    }
    
    public static class CommentListExtensions
    {
        public static CommentList ToCommentList(this IList<string> list)
        {
            var res = new CommentList();
            if (list is CommentList commentList)
                foreach (var c in commentList.Comments)
                    res.Add(c.Comment, c.Spacing);
            else
                foreach (var l in list) res.Add(l);
            return res;
        }
    }
}