namespace SshTools.Parent.Match.Criteria
{

    public class SingleCriteria : Criteria
    {
        public SingleCriteria(string name, MatchingFunc matchingFunc, CheckFunc checkFunc)
            : base(name, matchingFunc, checkFunc)
        {
        }
    }
}