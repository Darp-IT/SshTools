using System.Collections.Generic;
using System.Linq;
using FluentResults;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;

namespace SshTools
{
    public class SshToolsSettings
    {
        public const int MaxHostCharacters = 1025;
        
        
        private readonly IDictionary<string, Keyword> _keywordDict = new Dictionary<string, Keyword>();
        private readonly IDictionary<char, Token> _tokenDict = new Dictionary<char, Token>();
        private readonly IDictionary<string, Criteria> _criteriaDict = new Dictionary<string, Criteria>();

        public SshToolsSettings()
        {
            this.SetKeywords(Keyword.Values)
                .SetTokens(Token.Values)
                .SetCriteria(Criteria.Values);
        }

        
        //-----------------------------------------------------------------------//
        //                                Keywords
        //-----------------------------------------------------------------------//

        /// <summary>
        /// Sets an array of keyword by overwriting the current settings
        /// </summary>
        /// <param name="keywords">List of Keywords</param>
        /// <returns><see cref="SshToolsSettings"/></returns>
        public SshToolsSettings SetKeywords(params Keyword[] keywords)
        {
            _keywordDict.Clear();
            foreach (var keyword in keywords) _keywordDict[keyword.Name.ToUpper()] = keyword;
            return this;
        }

        /// <summary>
        /// Determines, whether a <see cref="Keyword"/> for the given, case invariant key is available
        /// </summary>
        /// <param name="keyword">The key to look for</param>
        /// <returns>true, if key is known</returns>
        public bool HasKeyword(string keyword) => _keywordDict.ContainsKey(keyword.ToUpper());
        
        /// <summary>
        /// Gets the <see cref="Keyword"/> for the given, case invariant key
        /// </summary>
        /// <param name="keyword">The key to be looked for</param>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
        /// The property is retrieved and <paramref name="keyword" /> is not found.
        /// </exception>
        /// <returns>The keyword </returns>
        public Keyword GetKeyword(string keyword) => _keywordDict[keyword.ToUpper()];
        
        internal Result<Keyword<T>> GetKeyword<T>()
        {
            var keyword = _keywordDict.Values
                .OfType<Keyword<T>>()
                .FirstOrDefault();
            return keyword != default 
                ? Result.Ok(keyword) 
                : Result.Fail<Keyword<T>>("Keyword not defined!");
        }

        //-----------------------------------------------------------------------//
        //                                Tokens
        //-----------------------------------------------------------------------//
        
        public SshToolsSettings SetTokens(params Token[] tokens)
        {
            _tokenDict.Clear();
            return AddTokens(tokens);
        }
        public SshToolsSettings AddTokens(params Token[] tokens)
        {
            foreach (var token in tokens) _tokenDict[token.Key] = token;
            return this;
        }
        public bool HasToken(char token) => _tokenDict.ContainsKey(token);
        
        /// <summary>
        /// Gets the <see cref="Token"/> for the given char
        /// </summary>
        /// <param name="token">The char to be looked for</param>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
        /// The property is retrieved and <paramref name="token" /> is not found.
        /// </exception>
        /// <returns>The Token </returns>
        public Token GetToken(char token) => _tokenDict[token];
        
        //-----------------------------------------------------------------------//
        //                                Criteria
        //-----------------------------------------------------------------------//
        
        public SshToolsSettings SetCriteria(params Criteria[] criteria)
        {
            _criteriaDict.Clear();
            return AddCriteria(criteria);
        }
        public SshToolsSettings AddCriteria(params Criteria[] criteria)
        {
            foreach (var c in criteria) _criteriaDict[c.Name] = c;
            return this;
        }
        public bool HasCriteria(string str) => _criteriaDict.ContainsKey(str);
        public Criteria GetCriteria(string str) => _criteriaDict[str];
    }
}