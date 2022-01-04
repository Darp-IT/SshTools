using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SshTools.Config.Parameters;
using SshTools.Config.Util;

namespace SshTools.Config.Parents
{
    public abstract class ParameterParent : IList<IParameter>, IConfigSerializable, ICloneable, IConnectable
    {
        //-----------------------------------------------------------------------//
        //                   ParameterParent Keyword Properties
        //-----------------------------------------------------------------------//
        
        public string User { get => this.Get(Keyword.User); set => this.Set(Keyword.User, value); }
        public string IdentityFile { get => this.Get(Keyword.IdentityFile); set => this.Set(Keyword.IdentityFile, value); }
        public ushort Port { get => this.Get(Keyword.Port); set => this.Set(Keyword.Port, value); }
        public string HostName { get => this.Get(Keyword.HostName); set => this.Set(Keyword.HostName, value); }
        public bool IdentitiesOnly { get => this.Get(Keyword.IdentitiesOnly); set => this.Set(Keyword.IdentitiesOnly, value); }

        
        
        //-----------------------------------------------------------------------//
        //                          ParameterParent Logic
        //-----------------------------------------------------------------------//
        
        private IList<IParameter> Params { get; }
        protected IParameter Parameter { get; set; }
        private readonly CommentList _commentsBacking = new CommentList();

        internal ParameterParent(IList<IParameter> parameters = null) => 
            Params = parameters ?? new List<IParameter>();

        public void Connect(IParameter param)
        {
            Parameter = param;
            if (_commentsBacking.Count <= 0) return;
            Parameter.Comments.Clear();
            foreach (var c in _commentsBacking.Comments) 
                Parameter.Comments.Add(c.Comment, c.Spacing);
        }
        public void Disconnect() => Parameter = null;
        public CommentList Comments => IsConnected
            ? Parameter.Comments
            : _commentsBacking;
        public bool IsConnected => Parameter != null;
        
        public object this[Keyword keyword] => this.Get(keyword);
        public virtual string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT) => 
            string.Join(Environment.NewLine, this.Select(p => p.Serialize(options)));
        public abstract object Clone();

        
        //-----------------------------------------------------------------------//
        //                          IList Implementation
        //-----------------------------------------------------------------------//
        
        public int Count => Params.Count;
        public bool IsReadOnly => false;
        
        public IEnumerator<IParameter> GetEnumerator() => Params.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void IfValidItem(IParameter item, Action action)
        {
            if (item == null) return;
            if (this is Node && item.Keyword.IsNode())
                throw new Exception($"Invalid keyword {item.Keyword} cannot be added to parent {GetType().Name}!");
            action();
        }
        public void Add(IParameter item) =>
            IfValidItem(item, () => Params.Add(item));

        public void Insert(int index, IParameter item) => 
            IfValidItem(item, () => Params.Insert(index, item));
        public void Clear() => Params.Clear();
        public bool Contains(IParameter item) => Params.Contains(item);
        public void CopyTo(IParameter[] array, int arrayIndex) => Params.CopyTo(array, arrayIndex);
        public bool Remove(IParameter item) => Params.Remove(item);
        public int IndexOf(IParameter item) => Params.IndexOf(item);
        public void RemoveAt(int index) => Params.RemoveAt(index);
        public IParameter this[int index]
        {
            get => Params[index];
            set => Params[index] = value;
        }
    }
}