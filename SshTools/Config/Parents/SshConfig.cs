using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentResults;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Util;

namespace SshTools.Config.Parents
{
    public class SshConfig : ParameterParent
    {
        //-----------------------------------------------------------------------//
        //                         Static Members getters
        //-----------------------------------------------------------------------//
        
        /// <summary>
        /// Parses a file by path.
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns><see cref="Result{TValue}"/> of type <see cref="SshConfig"/></returns>
        public static Result<SshConfig> ReadFile(string path)
        {
            var readRes = Result.Try(() => File.ReadAllText(path));
            return readRes.IsFailed
                ? readRes.ToResult<SshConfig>()
                : DeserializeString(readRes.Value, path);
        }
        /// <summary>
        /// Parses the SSH config text.
        /// </summary>
        /// <param name="str">Config string</param>
        /// <param name="fileName">An optional path that will be provided to the config for serialization</param>
        /// <returns><see cref="Result{TValue}"/> of type <see cref="SshConfig"/></returns>
        public static Result<SshConfig> DeserializeString(string str, string fileName = null) => 
            Result.Try(() => str.Deserialized().ToConfig(fileName));
        
        //-----------------------------------------------------------------------//
        //                          SshConfig class
        //-----------------------------------------------------------------------//
        public string FileName { get; }

        /// <summary>
        /// Create a new SshConfig
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="parameters"></param>
        /// <exception cref="ArgumentException">Throws argument exception, if there is no HOST/MATCH keyword defined</exception>
        public SshConfig(string fileName = null, IList<ILine> parameters = null)
            : base(parameters)
        {
            FileName = fileName;
        }
        
        public new string Serialize(SerializeConfigOptions options = SerializeConfigOptions.DEFAULT)
        {
            var lines = new List<string>();
            lines.AddRange(this.Select(p => p.Serialize(options)));
            return string.Join(Environment.NewLine, lines);
        }

        public override object Clone()
        {
            var parent = new SshConfig();
            foreach (var parameter in this) 
                parent.Add(parameter.Clone());
            return parent;
        }

        //-----------------------------------------------------------------------//
        //                   SshConfig specific functionality
        //-----------------------------------------------------------------------//
        
        /// <summary>
        /// Getter only, gets first host by looking for <paramref name="hostName"/>
        /// </summary>
        /// <param name="hostName">The name of the Host to be searched for</param>
        public HostNode this[string hostName] => this.Get(hostName);
        
        /// <summary>
        /// Gets a list of references to all <see cref="Node"/>s including the <see cref="SshConfig"/> at index [0]
        /// </summary>
        /// <param name="name">The name to be searched by</param>
        /// <param name="options">Searching options</param>
        /// <returns>A list of matching nodes with the config at position 0</returns>
        public IList<ParameterParent> GetAll(string name, MatchingOptions options = MatchingOptions.MATCHING)
        {
            var list = new List<ParameterParent> { this };
            foreach (var parameter in this.Matching(name, options))
            {
                if (parameter.Argument is Node parent)
                    list.Add(parent);
            }
            return list;
        }
        
        /// <summary>
        /// Gets a list of all nodes as reference including <see cref="SshConfig"/> at index [0]
        /// </summary>
        /// <returns>List of nodes</returns>
        public IList<ParameterParent> GetAll()
        {
            var list = new List<ParameterParent> { this};
            list.AddRange(this.Nodes());
            return list;
        }
    }
}