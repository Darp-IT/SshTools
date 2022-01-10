using System;
using System.Collections.Generic;
using FluentResults;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Util;

namespace SshTools.Config.Parents
{
    public abstract class Node : ParameterParent, IConnectable
    {
        protected Node(IList<ILine> parameters = null)
            : base(parameters)
        {
            
        }

        public abstract string Name { get; }
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
        
        public override string ToString() => Name;

        public override string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT)
        {
            var serializedBase = base.Serialize(options);
            return string.IsNullOrEmpty(serializedBase)
                ? Name
                : Name + Environment.NewLine + serializedBase;
        }

        internal abstract Result<ILine> GetParam();
        internal abstract Node Copy();

        public abstract bool Matches(string search, MatchingContext context, MatchingOptions options);
    }
}