using System.Collections.Generic;
using SshTools.Config.Parameters;

namespace SshTools.Config.Parents
{
    public static class ParameterParentListExtensions
    {
        /// <summary>
        /// Takes a sequence of <see cref="ParameterParent"/>
        /// and puts them into a single Host with name <see cref="hostName"/>.
        /// The parameters of the created host will be the same of the given sequence!
        /// Comments of all parents will be added if they contain anything
        /// </summary>
        /// <param name="included">A sequence of parents</param>
        /// <param name="hostName">The name of the created Host</param>
        /// <returns>The new Host</returns>
        public static HostNode ToHost(this IEnumerable<ParameterParent> included, string hostName)
        {
            var host = new HostNode(hostName);
            foreach (var parent in included)
            {
                foreach (var parentComment in parent.Comments)
                    if (!string.IsNullOrWhiteSpace(parentComment))
                        host.Comments.Add(parentComment);
                foreach (var line in parent)
                {
                    if (!(line is IParameter param)
                        || param.Keyword.AllowMultiple || !host.Has(param.Keyword))
                        host.Add(line);
                }
            }
            return host;
        }
    }
}