namespace SshTools.Parent.Match.Criteria
{
    public partial class Criteria
    {
        public delegate bool MatchingFunc(string search, MatchingContext context);
        
        public string Name { get; }
        private readonly MatchingFunc _matchingFunc;
        
        protected Criteria(string name, MatchingFunc matchingFunc)
        {
            _matchingFunc = matchingFunc;
            Name = name.ToLower();
        }

        public bool Matches(string search, MatchingContext context) => _matchingFunc(search, context);
        
        public override string ToString() => Name;
    }
}