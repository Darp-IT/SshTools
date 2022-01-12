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

        public SshToolsSettings Add<T>(params T[] values)
            where T : ISetting
        {
            var type = typeof(T);
            if (!_dict.ContainsKey(type))
                _dict[type] = new Dictionary<object, object>();
            
            foreach (var value in values) 
                _dict[type][value.Key] = value;
            return this;
        }
        
        public SshToolsSettings Set<T>(params T[] values)
            where T : ISetting
        {
            var type = typeof(T);
            if (_dict.ContainsKey(type))
                _dict[type].Clear();
            return Add(values);
        }

        public bool Has<T, TKey>(TKey key)
            where T : ISetting<TKey>
        {
            var type = typeof(T);
            return _dict.ContainsKey(type) && _dict[type].ContainsKey(key);
        }

        public T Get<T, TKey>(TKey key)
            where T : ISetting<TKey>
        {
            var type = typeof(T);
            if (!_dict.ContainsKey(type))
                throw new KeyNotFoundException($"Could not find key {key}! " +
                                               $"Given type {type.Name} has not been registered!");
            var tDict = _dict[type];
            if (!tDict.ContainsKey(key))
                throw new KeyNotFoundException($"Could not find given key {key} in settings");
            return (T)tDict[key];
        }
        
        public Result<T> Get<T>()
            where T : ISetting
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
        
        public bool Has<T>(string key) where T : ISetting<string> => Has<T, string>(key);
        public bool Has<T>(char key) where T : ISetting<char> => Has<T, char>(key);
        public T Get<T>(string key) where T : ISetting<string> => Get<T, string>(key);
        public T Get<T>(char key) where T : ISetting<char> => Get<T, char>(key);
    }
}