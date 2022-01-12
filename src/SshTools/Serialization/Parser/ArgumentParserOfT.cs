using FluentResults;

namespace SshTools.Serialization.Parser
{
    public class ArgumentParser<T>
    {
        public delegate Result<T> DeserializerFunc(string str);
        public delegate string SerializerFunc(T value, SerializeConfigOptions options);
        public delegate bool ParserFunc(string str, out T t);
        public DeserializerFunc Deserializer { get; }
        public SerializerFunc Serializer { get; }
        public string[] PossibleValues { get; }

        public ArgumentParser(DeserializerFunc deserializer, SerializerFunc serializer, string[] possibleValues = null)
        {
            Deserializer = str => str == null
                ? Result.Fail<T>("Could not parse input str of type null")
                : deserializer(str);
            Serializer = serializer;
            PossibleValues = possibleValues;
        }
        
        public ArgumentParser(ParserFunc parser) 
            : this(str => Parse(parser, str), (value, options) => value.ToString())
        {
        }

        private static Result<T> Parse(ParserFunc func, string value)
        {
            return func.Invoke(value, out var o) 
                ? Result.Ok(o) 
                : Result.Fail<T>($"Could not parse {value} to {typeof(T).Name}");
        }
    }
}