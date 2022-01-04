using System.Linq;
using FluentResults;
using SshTools.Config.Parents;
using SshTools.Config.Parser;
using SshTools.Config.Util;

namespace SshTools.Config.Parameters
{
    public abstract class Keyword
    {
        //TODO change IsHost to check if return type is of Type HostNode ...
        public static readonly Keyword<HostNode> Host = 
            new Keyword<HostNode>(nameof(Host), ArgumentParser.Host, allowMultiple:true);
        public static readonly Keyword<MatchNode> Match = 
            new Keyword<MatchNode>(nameof(Match), ArgumentParser.Match, allowMultiple:true);
        public static readonly Keyword<SshConfig> Include = 
            new Keyword<SshConfig>(nameof(Include), ArgumentParser.SshConfig, allowMultiple:true);

        public static readonly Keyword<string> User = 
            new Keyword<string>(nameof(User), ArgumentParser.String);
        public static readonly Keyword<string> HostName = 
            new Keyword<string>(nameof(HostName), ArgumentParser.String);
        public static readonly Keyword<string> ProxyJump = 
            new Keyword<string>(nameof(ProxyJump), ArgumentParser.String);
        public static readonly Keyword<string> IdentityFile = 
            new Keyword<string>(nameof(IdentityFile), ArgumentParser.String, allowMultiple:true);
        public static readonly Keyword<ushort> Port = 
            new Keyword<ushort>(nameof(Port), ArgumentParser.UShort);
        public static readonly Keyword<bool> IdentitiesOnly = 
            new Keyword<bool>(nameof(IdentitiesOnly), ArgumentParser.YesNo);
        
        /// <summary>
        /// The name in camel casing
        /// </summary>
        public string Name { get; }
        public abstract bool Is<TGeneric>(bool includeSubTypes = false);
        public bool AllowMultiple { get; }
        
        protected Keyword(string name, bool allowMultiple) =>
            (Name, AllowMultiple) = (name, allowMultiple);

        public static Keyword[] Values => typeof(Keyword)
            .GetFields()
            .Select(f => f.GetValue(null))
            .Where(v => v is Keyword)
            .Cast<Keyword>()
            .ToArray();

        public override string ToString() => Name;
        internal abstract Result<IParameter> GetParameter(string argument, ParameterAppearance appearance);
        internal abstract object GetDefault();
    }

    public class Keyword<T> : Keyword
    {
        private readonly ArgumentParser<T> _parser;
        public T Default { get; }
        public Keyword(string name, ArgumentParser<T> parser, T def = default,
            bool allowMultiple = false)
            : base(name, allowMultiple)
        {
            _parser = parser;
            Default = def;
        }
        
        internal Result<T> DeserializeArgument(string value) => _parser.Deserializer(value);
        internal string SerializeArgument(T value, SerializeConfigOptions options = SerializeConfigOptions.DEFAULT) => 
            _parser.Serializer(value, options);

        public override bool Is<TGeneric>(bool includeSubTypes = false)
        {
            var type1 = typeof(T);
            var type2 = typeof(TGeneric);
            return type1 == type2 
                   || includeSubTypes && type1.IsSubclassOf(type2);
        }

        internal override Result<IParameter> GetParameter(string argument, ParameterAppearance appearance)
        {
            var res = DeserializeArgument(argument);
            return res.IsFailed
                ? res.ToResult<IParameter>() 
                : Result.Ok<IParameter>(GetParam(res.Value, appearance));
        }

        internal override object GetDefault() => Default;

        internal IParameter<T> GetParam(T argument, ParameterAppearance appearance) =>
            new Parameter<T>(this, argument, appearance);
    }
}