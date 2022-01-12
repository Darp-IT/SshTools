using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using SshTools.Line;
using SshTools.Parent.Match.Criteria;
using SshTools.Serialization.Parser;

namespace SshTools.Parent.Match
{
    public class MatchNode : Node
    {
        //-----------------------------------------------------------------------//
        //                             Static Getter
        //-----------------------------------------------------------------------//
        
        /// <summary>
        /// Creates a new <see cref="MatchNode"/> from a given <see cref="SingleCriteria"/>.
        /// Might return null if setting fails
        /// </summary>
        /// <returns>New <see cref="MatchNode"/> or null</returns>
        public static Result<MatchNode> Of(string matchString)
        {
            var match = new MatchNode();
            var res = match.Parse(matchString);
            return res.IsSuccess ? Result.Ok(match) : res.ToResult<MatchNode>();
        }
        
        /// <summary>
        /// Creates a new <see cref="MatchNode"/> from a given <see cref="SingleCriteria"/>.
        /// Might return null if setting fails
        /// </summary>
        /// <param name="singleCriteria">A criteria this node will be initialized with</param>
        /// <returns>New <see cref="MatchNode"/> or null</returns>
        public static Result<MatchNode> Of(SingleCriteria singleCriteria)
        {
            var match = new MatchNode();
            var res = match.Set(singleCriteria);
            return res.IsSuccess ? Result.Ok(match) : res.ToResult<MatchNode>();
        }

        /// <summary>
        /// Creates a new <see cref="MatchNode"/> from a given <see cref="ArgumentCriteria"/>.
        /// Might return null if setting fails
        /// </summary>
        /// <param name="argumentCriteria">A criteria this node will be initialized with</param>
        /// <param name="argument">Argument(s) of the criteria</param>
        /// <returns>New <see cref="MatchNode"/> or null</returns>
        public static Result<MatchNode> Of(ArgumentCriteria argumentCriteria, params string[] argument)
        {
            var match = new MatchNode();
            var res = match.Set(argumentCriteria, argument);
            return res.IsSuccess ? Result.Ok(match) : res.ToResult<MatchNode>();
        }
        
        //-----------------------------------------------------------------------//
        //                             Class Content
        //-----------------------------------------------------------------------//
        
        // TODO removing / ... for match criteria

        public override string PatternName => string.Join("", _criteria.Select(c => c.ToString()));

        private readonly IList<CriteriaWrapper> _criteria = new List<CriteriaWrapper>();
        
        private MatchNode() { }
        private MatchNode(IList<ILine> parameters) : base(parameters) { }

        private Result<MatchNode> Parse(string matchString)
        {
            var parsingRes = MatchStringParser.Parse(matchString);
            if (parsingRes.IsFailed) return parsingRes.ToResult();
            foreach (var (criteria, spacing, value, spacingBack) in parsingRes.Value)
            {
                var res = SetCriteria(criteria, value, spacing, spacingBack);
                if (res.IsFailed) return res;
            }
            return Result.Ok(this);
        }

        public Result Set(SingleCriteria singleCriteria) => SetCriteria(singleCriteria);

        public Result Set(ArgumentCriteria argumentCriteria, params string[] values)
        {
            if (values is null || values.Length == 0)
                return Result.Fail($"Could not set criteria {argumentCriteria} to match! No values were provided");
            return values.All(string.IsNullOrWhiteSpace)
                ? Result.Fail($"Could not set criteria {argumentCriteria} to match! Only empty values were provided!")
                : SetCriteria(argumentCriteria, string.Join(",", values));
        }

        private Result SetCriteria(Criteria.Criteria singleCriteria, string value = null, string spacing = null, string spacingBack = null)
        {
            if (singleCriteria is null)
                return Result.Fail($"Could not set a criteria to match! Argument criteria must not be null");
            var maybeFirst = _criteria.FirstOrDefault(c => c.Type.Equals(singleCriteria));
            if (maybeFirst == default)
            {
                //TODO checks if the position is valid
                //TODO Some way to see if only valid tokens are specified
                //TODO I need quoted values
                _criteria.Add(new CriteriaWrapper(singleCriteria, spacing, value, spacingBack));
                return Result.Ok();
            }
            maybeFirst.Value = value;
            return Result.Ok();
        }
        
        public override bool Matches(string search, MatchingContext context, MatchingOptions options)
        {
            return options is MatchingOptions.EXACT
                ? search.Equals(PatternName)
                : _criteria.All(c => c.Type.Matches(search, context));
        }

        public override object Clone()
        {
            var node = new MatchNode(
                this
                    .Select(p => p is ICloneable cloneable 
                        ? (ILine) cloneable.Clone() 
                        : p)
                    .ToList()
            );
            node.Parse(PatternName);
            return node;
        }

        internal override Node Copy()
        {
            var res = new MatchNode().Parse(PatternName);
            if (res.IsFailed) throw new Exception("Could not copy node! " + string.Join(",", res.Errors));
            return res.Value;
        }
    }

    public class CriteriaWrapper
    {
        public Criteria.Criteria Type { get; }
        public string Spacing { get; }
        public string Value { get; set; }
        public string SpacingBack { get; }
        
        public CriteriaWrapper(Criteria.Criteria type, string spacing, string value, string spacingBack)
        {
            Type = type;
            Spacing = spacing;
            Value = value;
            SpacingBack = spacingBack;
        }

        public override string ToString() =>
            Type
            + (Spacing ?? (Value == null ? "" : " "))
            + (Value ?? "")
            + (SpacingBack ?? "");
    }
}