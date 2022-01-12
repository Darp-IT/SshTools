using System;
using System.Collections.Generic;
using SshTools.Line;
using SshTools.Line.Comment;
using SshTools.Line.Parameter;
using SshTools.Parent.Match;
using SshTools.Serialization;

namespace SshTools.Parent
{
    public abstract class Node : ParameterParent, IConnectable
    {
        protected Node(IList<ILine> parameters = null)
            : base(parameters)
        {
            
        }

        public abstract string PatternName { get; }
        private readonly CommentList _commentsBacking = new CommentList();
        private IParameter _parameter;
        
        public bool IsConnected => _parameter != null;
        public CommentList Comments => IsConnected
            ? _parameter.Comments
            : _commentsBacking;
        
        public void Connect(IParameter param)
        {
            _parameter = param;
            if (_commentsBacking.Count <= 0) return;
            _parameter.Comments.Clear();
            foreach (var c in _commentsBacking.Comments) 
                _parameter.Comments.Add(c);
        }
        public void Disconnect() => _parameter = null;
        
        public override string ToString() => PatternName;

        public override string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT)
        {
            var serializedBase = base.Serialize(options);
            return string.IsNullOrEmpty(serializedBase)
                ? PatternName
                : PatternName + Environment.NewLine + serializedBase;
        }

        internal abstract Node Copy();

        public abstract bool Matches(string search, MatchingContext context, MatchingOptions options);
    }
}