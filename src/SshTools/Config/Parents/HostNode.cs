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
        //-----------------------------------------------------------------------//
        //                             Static Getter
        //-----------------------------------------------------------------------//
        
        /// <summary>
        /// Instantiates a new HostNode.
        /// </summary>
        /// <param name="hostName">The name of the host</param>
        public static Result<HostNode> Of(string hostName)
        {
            if (hostName is null)
                return Result.Fail<HostNode>($"HostNode creation failed. Given name is null!");
            var res = LineParser.IsValidPattern(hostName);
            return res.IsSuccess
                ? Result.Ok(new HostNode(hostName))
                : Result.Merge(Result.Fail($"HostNode creation for name {hostName} failed"), res).ToResult<HostNode>();
        }
        
        //-----------------------------------------------------------------------//
        //                             Class Content
        //-----------------------------------------------------------------------//
        
        // TODO rename functionality for HostNodes, removing / ... for match criteria
        
        private HostNode(string patternName) => PatternName = patternName;
        private HostNode(string patternName, IList<ILine> parameters) : base(parameters) => PatternName = patternName;
        
        public override string PatternName { get; }
        
        public override object Clone()
        {
            return new HostNode(
                PatternName, 
                this
                    .Select(p => p is ICloneable cloneable 
                        ? (ILine) cloneable.Clone() 
                        : p)
                    .ToList()
                );
        }

        internal override Node Copy() => new HostNode(PatternName);
        public override bool Matches(string search, MatchingContext context, MatchingOptions options) =>
            options == MatchingOptions.EXACT
                ? PatternName.Equals(search)
                : Globber.Glob(PatternName, search);
    }
}