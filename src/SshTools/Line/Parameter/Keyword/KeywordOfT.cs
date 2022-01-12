using FluentResults;
using SshTools.Serialization;
using SshTools.Serialization.Parser;

namespace SshTools.Line.Parameter.Keyword
{
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