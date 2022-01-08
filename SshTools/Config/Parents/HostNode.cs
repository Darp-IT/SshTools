using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Parser;

namespace SshTools.Config.Parents
{
    public class HostNode : Node
    {
        public HostNode(string matchString, IList<ILine> parameters = null)
            : base(parameters)
        {
            MatchString = matchString;
        }

        public override string MatchString { get; }
        
        public override object Clone()
        {
            return new HostNode(
                MatchString, 
                this
                    .Select(p => p is ICloneable cloneable 
                        ? (ILine) cloneable.Clone() 
                        : p)
                    .ToList()
                );
        }


        internal override Result<ILine> GetParam()
        {
            var res = SshTools.Settings.GetKeyword<HostNode>();
            return res.IsFailed
                ? res.ToResult<ILine>()
                : Result.Ok<ILine>(res.Value.GetParam(this, ParameterAppearance.Default(res.Value)));
        }

        internal override Node Copy() => new HostNode(MatchString);
        public override bool Matches(string search, MatchingContext context, MatchingOptions options) =>
            options == MatchingOptions.EXACT
                ? MatchString.Equals(search)
                : Globber.Glob(MatchString, search);
    }
}