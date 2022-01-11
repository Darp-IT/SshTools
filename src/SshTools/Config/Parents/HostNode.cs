using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using SshTools.Config.Extensions;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Parser;

namespace SshTools.Config.Parents
{
    public class HostNode : Node
    {
        // TODO rename functionality for HostNodes, removing / ... for match criteria
        
        /// <summary>
        /// Instantiates a new HostNode.
        /// </summary>
        /// <param name="hostName">The name of the host</param>
        /// <param name="parameters">Optional list of all parameters the host will contain</param>
        /// <exception cref="ArgumentException"><paramref name="hostName"/> must not be null or whiteSpace</exception>
        public HostNode(string hostName, IList<ILine> parameters = null)
            : base(parameters)
        {
            hostName.ThrowIfNullOrEmpty();
            Name = hostName;
        }

        public override string Name { get; }
        
        public override object Clone()
        {
            return new HostNode(
                Name, 
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

        internal override Node Copy() => new HostNode(Name);
        public override bool Matches(string search, MatchingContext context, MatchingOptions options) =>
            options == MatchingOptions.EXACT
                ? Name.Equals(search)
                : Globber.Glob(Name, search);
    }
}