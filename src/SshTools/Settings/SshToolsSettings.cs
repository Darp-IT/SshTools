using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace SshTools.Settings
{
    public class SshToolsSettings
    {
        public const int MaxHostCharacters = 1025;

        private readonly IDictionary<Type, IDictionary<object, object>> _dict =
            new Dictionary<Type, IDictionary<object, object>>();

        /// <summary>
        /// Adds <paramref name="values"/> to the settings group of <typeparamref name="T"/>
        /// </summary>
        /// <param name="values">The values to be added</param>
        /// <typeparam name="T">The type of the group to be added to</typeparam>
        /// <returns><see cref="SshToolsSettings"/></returns>
        public SshToolsSettings Add<T>(params T[] values)
            where T : IKeyedSetting
        {
            foreach (var value in values)
            {
                var type = value.Type;
                if (!_dict.ContainsKey(type))
                    _dict[type] = new Dictionary<object, object>();
                _dict[type][value.Key] = value;
            }
            return this;
        }
        
        /// <summary>
        /// Clears settings group if existent
        /// </summary>
        /// <typeparam name="T">The group to be cleared</typeparam>
        /// <returns><see cref="SshToolsSettings"/></returns>
        public SshToolsSettings Clear<T>()
            where T : IKeyedSetting
        {
            var type = typeof(T);
            if (_dict.ContainsKey(type))
                _dict[type].Clear();
            return this;
        }

        /// <summary>
        /// Checks if a item inside a settings group is present
        /// </summary>
        /// <param name="key">The key to be looked for</param>
        /// <typeparam name="T">The type of the group to be checked</typeparam>
        /// <typeparam name="TKey">The type of the <paramref name="key"/></typeparam>
        /// <returns>Whether the key is contained in the given group</returns>
        public bool Has<T, TKey>(TKey key)
            where T : IKeyedSetting<TKey>
        {
            var type = typeof(T);
            return _dict.ContainsKey(type) && _dict[type].ContainsKey(key);
        }

        /// <summary>
        /// Gets the item of type <typeparamref name="T"/> if the item inside a settings group is present
        /// </summary>
        /// <param name="key">The key to be looked for</param>
        /// <typeparam name="T">The type of the group to be checked</typeparam>
        /// <typeparam name="TKey">The type of the <paramref name="key"/></typeparam>
        /// <returns>Whether the key is contained in the given group</returns>
        /// <exception cref="KeyNotFoundException">Thrown if group or key are unknown</exception>
        public T Get<T, TKey>(TKey key)
            where T : IKeyedSetting<TKey>
        {
            var type = typeof(T);
            if (!_dict.ContainsKey(type))
                throw new KeyNotFoundException($"Could not find key '{key}'! " +
                                               $"Given type '{type.Name}' has not been registered!");
            var tDict = _dict[type];
            if (!tDict.ContainsKey(key))
                throw new KeyNotFoundException($"Could not find given key '{key}' in settings" +
                                               $" for given type '{type.Name}'");
            return (T)tDict[key];
        }
        
        /// <summary>
        /// Gets the first item, that matches the given type in all groups
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Result<T> Get<T>()
            where T : IKeyedSetting
        {
            var type = typeof(T);
            foreach (var keyValuePair in _dict)
            {
                if (!type.IsSubclassOf(keyValuePair.Key)) continue;
                var res = keyValuePair.Value.Values
                    .OfType<T>()
                    .FirstOrDefault();
                if (res != null)
                    return Result.Ok(res);
            }
            return Result.Fail<T>($"Unknown Key of Type '{type.Name}'");
        }
        
        /// <seealso cref="Has{T,TKey}"/>
        public bool Has<T>(string key) where T : IKeyedSetting<string> => Has<T, string>(key);
        /// <seealso cref="Has{T,TKey}"/>
        public bool Has<T>(char key) where T : IKeyedSetting<char> => Has<T, char>(key);
        /// <seealso cref="Get{T,TKey}"/>
        public T Get<T>(string key) where T : IKeyedSetting<string> => Get<T, string>(key);
        /// <seealso cref="Get{T,TKey}"/>
        public T Get<T>(char key) where T : IKeyedSetting<char> => Get<T, char>(key);
    }
}