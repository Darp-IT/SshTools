using System;
using System.Collections.Generic;
using FluentResults;
using SshTools.Settings;

namespace SshTools.Parent.Match.Criteria
{
    public partial class Criteria : IKeyedSetting<string>
    {
        public delegate bool MatchingFunc(string search, MatchingContext context);
        public delegate Result CheckFunc(IList<Criteria> criteria);
        
        public string Name { get; }
        private readonly MatchingFunc _matchingFunc;
        private readonly CheckFunc _checkFunc;
        
        protected Criteria(string name, MatchingFunc matchingFunc, CheckFunc checkFunc)
        {
            _matchingFunc = matchingFunc;
            _checkFunc = checkFunc;
            Name = name.ToLower();
        }

        public bool Matches(string search, MatchingContext context) => _matchingFunc(search, context);
        
        public override string ToString() => Name;
        object IKeyedSetting.Key => Name;
        Type IKeyedSetting.Type => typeof(Criteria);
        string IKeyedSetting<string>.Key => Name;
        public Result Check(IList<Criteria> toArray) => _checkFunc(toArray);
    }
}