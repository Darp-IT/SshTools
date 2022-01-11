using SshTools.Config.Parents;

namespace SshTools.Config.Matching
{
    public enum MatchingOptions
    {
        /// <summary>
        /// Search will return only parameters with exactly matching <see cref="Node.Name"/>
        /// </summary>
        EXACT,
        
        /// <summary>
        /// Will return
        /// </summary>
        PATTERN
    }
}