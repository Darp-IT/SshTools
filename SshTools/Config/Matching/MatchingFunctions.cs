using SshTools.Config.Parser;

namespace SshTools.Config.Matching
{
    public static class MatchingFunctions
    {
        public static bool MatchesKeyName<T>(string keyName, string search, MatchingContext context)
        {
            if (!context.HasProperty<T>(keyName))
                return false;
            var s = context.GetProperty<T>(keyName);
            var expandRes = context.Expand(search);
            return expandRes.IsSuccess
                   && Globber.Glob(expandRes.Value, s.ToString());
        }

    }
}