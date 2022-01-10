using System;
using FluentResults;
using SshTools.Config.Parameters;
using SshTools.Config.Parents;
using SshTools.Config.Util;

namespace SshTools.Config.Parser
{
    public static class ArgumentParser
    {
        public static readonly ArgumentParser<HostNode> Host = new ArgumentParser<HostNode>(
            str => Result.Ok(new HostNode(str)),
            (value, options) => value.Serialize(options)
        );
        
        public static readonly ArgumentParser<MatchNode> Match = new ArgumentParser<MatchNode>(
            str => new MatchNode().Parse(str),
            (value, options) => value.Serialize(options)
        );
        
        public static readonly ArgumentParser<SshConfig> SshConfig = new ArgumentParser<SshConfig>(
            Parents.SshConfig.ReadFile,
            (value, options) => value.FileName ?? value.Serialize(options)
        );
        
        public static readonly ArgumentParser<string> String = new ArgumentParser<string>(
            Result.Ok, 
            (value, options) => value
            );
        
        public static readonly ArgumentParser<ushort> UShort = new ArgumentParser<ushort>(ushort.TryParse);
        
        public static readonly ArgumentParser<bool> YesNo = new ArgumentParser<bool>(
            str =>
            {
                switch (str.ToLower())
                {
                    case "yes":
                        return Result.Ok(true);
                    case "no":
                        return Result.Ok(false);
                    default:
                        return Result.Fail<bool>($"Could not parse argument. Got invalid invalid input string '{str}'!");
                }
            }, 
            (value, options) => value ? "yes" : "no",
            new []{ "no", "yes" });
    }

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