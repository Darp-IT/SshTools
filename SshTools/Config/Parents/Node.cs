using System;
using System.Collections.Generic;
using FluentResults;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Util;

namespace SshTools.Config.Parents
{
    public abstract class Node : ParameterParent
    {
        protected Node(IList<IParameter> parameters = null)
            : base(parameters)
        {
            
        }

        public abstract string MatchString { get; }
        public override string ToString() => MatchString;

        public override string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT)
        {
            var serializedBase = base.Serialize(options);
            return string.IsNullOrEmpty(serializedBase)
                ? MatchString
                : MatchString + Environment.NewLine + serializedBase;
        }

        internal abstract Result<IParameter> GetParam();
        internal abstract Node Copy();

        public abstract bool Matches(string search, MatchingContext context, MatchingOptions options);
    }
}