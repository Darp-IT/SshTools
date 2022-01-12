using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using SshTools.Line;
using SshTools.Parent.Match;
using SshTools.Serialization.Parser;

namespace SshTools.Parent.Host
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
            var res = LineParser.IsValidPattern(hostName);
            return res.IsSuccess
                ? Result.Ok(new HostNode(hostName))
                : Result.Merge(Result.Fail($"HostNode creation for name {hostName} failed"), res).ToResult<HostNode>();
        }
        
        //-----------------------------------------------------------------------//
        //                             Class Content
        //-----------------------------------------------------------------------//
        
        private HostNode(string patternName) => _patternName = patternName;
        private HostNode(string patternName, IList<ILine> parameters) : base(parameters) => _patternName = patternName;

        private string _patternName;
        public override string PatternName => _patternName;

        /// <summary>
        /// Enter a new pattern as name for the <see cref="HostNode"/>.
        /// Will fail, if the pattern was invalid
        /// </summary>
        /// <param name="newPatternName">The pattern to be set</param>
        /// <returns>Result, whether the pattern was set</returns>
        public Result Rename(string newPatternName)
        {
            var res = LineParser.IsValidPattern(newPatternName);
            if (res.IsSuccess)
                _patternName = newPatternName;
            return res;
        }
        
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