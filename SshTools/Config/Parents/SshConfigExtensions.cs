using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using SshTools.Config.Extensions;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;

namespace SshTools.Config.Parents
{
    public static class ParentExtensions
    {
        public static bool Has(this SshConfig sshConfig, string hostName, MatchingOptions options = MatchingOptions.EXACT) =>
            sshConfig.Matching(hostName, options).WhereArg<Node>().Any();

        public static int IndexOf(this SshConfig sshConfig, string hostName)
        {
            for (var i = 0; i < sshConfig.Count; i++)
            {
                if (!(sshConfig[i] is IParameter param)) continue;
                if (param.Argument is Node node && node.MatchString.Equals(hostName))
                    return i;
            }
            return -1;
        }
        
        public static HostNode Get(this SshConfig sshConfig, string hostName) => sshConfig
            .Matching(hostName, MatchingOptions.EXACT)
            .WhereArg<HostNode>()
            .SelectArg()
            .FirstOrDefault();

        /// <summary>
        /// Returns a list of all matching nodes including the <paramref name="config"/> at index [0]
        /// </summary>
        /// <param name="config">The config to be searched</param>
        /// <param name="name">The name to be searched by</param>
        /// <param name="options">Searching options</param>
        /// <returns>A list of matching nodes</returns>
        public static IList<Node> GetAll(
            this SshConfig config,
            string name,
            MatchingOptions options = MatchingOptions.MATCHING)
        {
            var list = new List<Node> { config.FirstToHost("") };
            foreach (var parameter in config.Matching(name, options))
            {
                if (parameter.Argument is Node parent)
                    list.Add(parent);
            }
            return list;
        }
        
        /// <summary>
        /// Gets a list of all nodes including the <paramref name="config"/> at index [0]
        /// </summary>
        /// <param name="config">The config to be searched</param>
        /// <returns>List of nodes</returns>
        public static IList<Node> GetAll(this SshConfig config)
        {
            var list = new List<Node> {config.FirstToHost("")};
            list.AddRange(config.Nodes());
            return list;
        }
        
        /// <summary>
        /// Compiles all matching parameters of a config into a unique host.
        /// The host will be completely uncoupled from the config.
        /// </summary>
        /// <param name="sshConfig">The <see cref="SshConfig"/> to be searched</param>
        /// <param name="hostName">The HostName to be searched for</param>
        /// <param name="options">Matching options for the <see cref="hostName"/></param>
        /// <returns>The new Host</returns>
        public static HostNode Find(this SshConfig sshConfig,
            string hostName,
            MatchingOptions options = MatchingOptions.MATCHING) =>
            sshConfig
                .Compiled()
                .Matching(hostName, options)
                .WhereArg<Node>()
                .SelectArg()
                .ToHost(hostName);

        public static Result<T> Insert<T>(this SshConfig sshConfig, int index, Keyword<T> keyword, string name)
            where T : Node
        {
            var desRes = keyword.DeserializeArgument(name);
            return desRes.IsSuccess
                ? sshConfig.Insert(index, keyword, desRes.Value)
                : desRes.ToResult();
        }
        
        public static Result<T> Set<T>(this SshConfig sshConfig, Keyword<T> keyword, string name)
            where T : Node
        {
            var desRes = keyword.DeserializeArgument(name);
            return desRes.IsSuccess
                ? sshConfig.Set(keyword, desRes.Value)
                : desRes;
        }

        public static void Remove(this SshConfig config, Func<ILine, bool> func, int maxCount = int.MaxValue)
        {
            config.ThrowIfNull();
            func.ThrowIfNull();
            var i = 0;
            var count = 0;
            while (i < config.Count && count < maxCount)
            {
                if (func(config[i]))
                {
                    config.RemoveAt(i);
                    count++;
                }
                else
                {
                    i++;
                }
            }
        }

        public static void Remove<T>(this SshConfig config, Func<IArgParameter<T>, bool> func, int maxCount = int.MaxValue) =>
            config.Remove(p => p is IArgParameter<T> param && func(param), maxCount);

        public static void Remove(this SshConfig config, string name,
            int maxCount = int.MaxValue,
            MatchingOptions options = MatchingOptions.EXACT) =>
            config.Remove<Node>(p => p.Argument.Matches(name, new MatchingContext(name), options), maxCount);

        /// <summary>
        /// Creates a new SshConfig, with all includes
        /// </summary>
        /// <returns>New SshConfig</returns>
        public static SshConfig Compile(this SshConfig sshConfig) => sshConfig
            .Compiled()
            .ToConfig();
        
        public static IList<HostNode> Hosts(this SshConfig sshConfig) => sshConfig
            .WhereArg<HostNode>()
            .SelectArg()
            .ToList();
        
        public static IList<Node> Nodes(this SshConfig sshConfig) => sshConfig
            .WhereArg<Node>()
            .SelectArg()
            .ToList();

        public static IList<MatchNode> Matches(this SshConfig sshConfig) => sshConfig
            .WhereArg<MatchNode>()
            .SelectArg()
            .ToList();
        
        public static TP PushHost<TP>(this TP parent, string hostName, Action<HostNode> func = null)
            where TP : SshConfig
        {
            var res = parent.Insert(0, Keyword.Host, hostName);
            if (res.IsSuccess) func?.Invoke(res.Value);
            return parent;
        }
        
        public static TP PushMatch<TP>(this TP parent, Action<MatchNode> func = null)
            where TP : SshConfig
        {
            var res = parent.Insert(0, Keyword.Match, "");
            if (res.IsSuccess) func?.Invoke(res.Value);
            return parent;
        }
    }
}