using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Parser;

namespace SshTools.Config.Parents
{
    public class MatchNode : Node
    {
        public override string Name => string.Join("", _criteria.Select(c => c.ToString()));

        private readonly IList<CriteriaWrapper> _criteria = new List<CriteriaWrapper>();
        
        public MatchNode(IList<ILine> parameters = null)
            : base(parameters)
        {
            
        }

        public Result<MatchNode> Parse(string matchString)
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

        public Result Set(Criteria criteria) => 
            SetCriteria(criteria);

        public Result Set(ArgumentCriteria argumentCriteria, params string[] values)
        {
            if (values is null || values.Length == 0)
                return Result.Fail($"Could not set criteria {argumentCriteria} to match! No values were provided");
            return values.All(string.IsNullOrWhiteSpace)
                ? Result.Fail($"Could not set criteria {argumentCriteria} to match! Only empty values were provided!")
                : SetCriteria(argumentCriteria, string.Join(",", values));
        }

        private Result SetCriteria(Criteria criteria, string value = null, string spacing = null, string spacingBack = null)
        {
            if (criteria is null)
                return Result.Fail($"Could not set a criteria to match! Argument criteria must not be null");
            var maybeFirst = _criteria.FirstOrDefault(c => c.Type.Equals(criteria));
            if (maybeFirst == default)
            {
                //TODO checks if the position is valid
                //TODO Some way to see if only valid tokens are specified
                //TODO I need quoted values
                _criteria.Add(new CriteriaWrapper(criteria, spacing, value, spacingBack));
                return Result.Ok();
            }
            maybeFirst.Value = value;
            return Result.Ok();
        }
        
        public override bool Matches(string search, MatchingContext context, MatchingOptions options)
        {
            return options is MatchingOptions.EXACT
                ? search.Equals(Name)
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
            node.Parse(Name);
            return node;
        }

        internal override Result<ILine> GetParam()
        {
            var res = SshTools.Settings.GetKeyword<MatchNode>();
            return res.IsFailed
                ? res.ToResult<ILine>()
                : Result.Ok<ILine>(res.Value.GetParam(this, ParameterAppearance.Default(res.Value)));
        }

        internal override Node Copy()
        {
            var res = new MatchNode().Parse(Name);
            if (res.IsFailed) throw new Exception("Could not copy node! " + string.Join(",", res.Errors));
            return res.Value;
        }
    }

    public class CriteriaWrapper
    {
        public Criteria Type { get; }
        public string Spacing { get; }
        public string Value { get; set; }
        public string SpacingBack { get; }
        
        public CriteriaWrapper(Criteria type, string spacing, string value, string spacingBack)
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