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
            SetKeywords(Keyword.Values);
            SetTokens(Token.Values);
            SetCriteria(Criteria.Values);
        }

        
        //-----------------------------------------------------------------------//
        //                                Keywords
        //-----------------------------------------------------------------------//

        public SshToolsSettings SetKeywords(params Keyword[] keywords)
        {
            _keywordDict.Clear();
            foreach (var keyword in keywords) _keywordDict[keyword.Name.ToUpper()] = keyword;
            return this;
        }

        public bool HasKeyword(string keyword) => _keywordDict.ContainsKey(keyword.ToUpper());
        public Keyword GetKeyword(string keyword) => _keywordDict[keyword];
        
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