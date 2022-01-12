namespace SshTools.Parent.Match.Criteria
{
    public class ArgumentCriteria : Criteria
    {
        public ArgumentCriteria(string name, MatchingFunc matchingFunc, CheckFunc checkFunc)
            : base(name, matchingFunc, checkFunc)
        {
        }
    }
}