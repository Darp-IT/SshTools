using System;
using System.Collections.Generic;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;

namespace SshTools.Config.Parents
{
    public static class SshConfigExtensions
    {
        /// <summary>
        /// Pushes a new <see cref="HostNode"/> to the given <typeparamref name="TLines"/> by inserting at the first place
        /// If there is an error during insertion it will be silently ignored and the <paramref name="func"/> wont be executed
        /// </summary>
        /// <param name="lines">The list of lines of type <typeparamref name="TLines"/> to be pushed to</param>
        /// <param name="hostName">The name of the new host</param>
        /// <param name="func">An optional function to further edit the newly created host</param>
        /// <typeparam name="TLines">The type of the list of <paramref name="lines"/></typeparam>
        /// <returns>The given list of <paramref name="lines"/> with the inserted host at position 0</returns>
        public static TLines PushHost<TLines>(this TLines lines, string hostName, Action<HostNode> func = null)
            where TLines : IList<ILine>
        {
            var res = lines.InsertHost(0, hostName);
            if (res.IsSuccess) func?.Invoke(res.Value);
            return lines;
        }
        
        /// <summary>
        /// Pushes a new <see cref="MatchNode"/> to the given <typeparamref name="TLines"/> by inserting at the first place
        /// If there is an error during insertion it will be silently ignored and the <paramref name="func"/> wont be executed
        /// </summary>
        /// <param name="lines">The list of lines of type <typeparamref name="TLines"/> to be pushed to</param>
        /// <param name="criteria">The criteria of the match</param>
        /// <param name="func">An optional function to further edit the newly created host</param>
        /// <typeparam name="TLines">The type of the list of <paramref name="lines"/></typeparam>
        /// <returns>The given list of <paramref name="lines"/> with the inserted match at position 0</returns>
        public static TLines PushMatch<TLines>(this TLines lines, Criteria criteria, Action<MatchNode> func = null)
            where TLines : IList<ILine>
        {
            var res = lines.InsertMatch(0, criteria);
            if (res.IsSuccess) func?.Invoke(res.Value);
            return lines;
        }

        /// <summary>
        /// Pushes a new <see cref="MatchNode"/> to the given <typeparamref name="TLines"/> by inserting at the first place
        /// If there is an error during insertion it will be silently ignored and the <paramref name="func"/> wont be executed
        /// </summary>
        /// <param name="lines">The list of lines of type <typeparamref name="TLines"/> to be pushed to</param>
        /// <param name="criteria">The criteria of the match</param>
        /// <param name="argument">The argument of <paramref name="criteria"/></param>
        /// <param name="func">An optional function to further edit the newly created host</param>
        /// <typeparam name="TLines">The type of the list of <paramref name="lines"/></typeparam>
        /// <returns>The given list of <paramref name="lines"/> with the inserted match at position 0</returns>
        public static TLines PushMatch<TLines>(this TLines lines, ArgumentCriteria criteria, string argument,
            Action<MatchNode> func = null)
            where TLines : IList<ILine>
        {
            var res = lines.InsertMatch(0, criteria, argument);
            if (res.IsSuccess) func?.Invoke(res.Value);
            return lines;
        }
    }
}