using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SshTools.Config.Parameters
{
    public class CommentList : IList<string>
    {
        internal IList<Comment> Comments { get; } = new List<Comment>();
        public IEnumerator<string> GetEnumerator() => Comments
            .Select(c => c.Argument).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal void Add(Comment item) => 
            Comments.Add(item);
        public void Add(string item) => 
            Add(new Comment(item));
        public void Add(string item, string spacing) => 
            Add(new Comment(item, spacing));
        internal void AddRange(IEnumerable<Comment> comments)
        {
            foreach (var comment in comments) Add(comment);
        }
        public void Clear() => Comments.Clear();

        public bool Contains(string item) => Comments
            .Select(c => c.Argument).Contains(item);

        public void CopyTo(string[] array, int arrayIndex) => Comments
            .CopyTo(array.Select(a => new Comment(a)).ToArray(), arrayIndex);

        public bool Remove(string item)
        {
            for (var i = Comments.Count - 1; i >= 0; i--)
            {
                if (!Comments[i].Argument.Equals(item)) continue;
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
                if (!Comments[i].Argument.Equals(item))
                    continue;
                return i;
            }
            return -1;
        }

        public void Insert(int index, string item) => 
            Comments.Insert(index, new Comment(item));

        public void RemoveAt(int index) => Comments.RemoveAt(index);

        public string this[int index]
        {
            get => Comments[index].Argument;
            set => Comments[index] = new Comment(value);
        }
    }
    
    public static class CommentListExtensions
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
        
        public static CommentList ToCommentList(this IEnumerable<Comment> list)
        {
            var res = new CommentList();
            res.AddRange(list);
            return res;
        }
    }
}