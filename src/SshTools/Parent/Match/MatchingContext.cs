using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FluentResults;
using SshTools.Serialization.Parser;

namespace SshTools.Parent.Match
{
    public class MatchingContext
    {
        private readonly IDictionary<string, object> _lookup = new Dictionary<string, object>();

        /// <summary>
        /// The HostName, that is being searched for (set once in the constructor)
        /// </summary>
        public string OriginalHostName
        {
            get => GetProperty<string>();
            private set => SetProperty(value);
        }
        
        /// <summary>
        /// The current Remote HostName
        /// </summary>
        public string HostName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        
        /// <summary>
        /// Optionally defined alias HostName
        /// </summary>
        public string HostKeyAlias
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        
        /// <summary>
        /// The current User
        /// </summary>
        public string User
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        
        /// <summary>
        /// The local user - set once in the constructor
        /// </summary>
        public string LocalUser
        {
            get => GetProperty<string>();
            private set => SetProperty(value);
        }
        
        //public string LocalUser => Environment.UserName;
        
        /// <summary>
        /// The current Port
        /// </summary>
        public ushort Port
        {
            get => GetProperty<ushort>();
            set => SetProperty(value);
        }
        
        /// <summary>
        /// Indicates, if this is the last parsing cycle
        /// </summary>
        public bool Last
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }
        
        public MatchingContext(string originalHost)
        {
            OriginalHostName = originalHost;
            LocalUser = Environment.UserName;
        }
        
        /// <summary>
        /// Expands a string by using the current context
        /// </summary>
        /// <param name="search">Search string</param>
        /// <returns><see cref="TokenParser.Parse"/></returns>
        public Result<string> Expand(string search) => TokenParser.Parse(search, this);

        public bool HasProperty<T>(string name)
        {    
            if (name == null) return false;
            name = name.ToLowerInvariant();
            return _lookup.ContainsKey(name) && _lookup[name] is T;
        }

        /// <summary>
        /// Sets a property by a certain string. The key will be case insensitive
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="name">Name of the key for retrieval</param>
        /// <returns>Whether the new value has been set</returns>
        public bool SetProperty(object value, [CallerMemberName] string name = null)
        {
            if (name == null) return false;
            name = name.ToLowerInvariant();
            _lookup[name] = value;
            return true;
        }
        
        /// <summary>
        /// Gets a property of a certain type. Will return default values if not present
        /// </summary>
        /// <param name="name">Name of key, case invariant</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>Value if type <typeparamref name="T"/> or default</returns>
        public T GetProperty<T>([CallerMemberName] string name = null)
        {
            if (name == null) return default;
            name = name.ToLowerInvariant();
            if (!HasProperty<T>(name)) return default;
            return (T) _lookup[name];
        }
    }
}