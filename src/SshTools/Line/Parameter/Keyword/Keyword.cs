using System;
using System.Linq;
using FluentResults;
using SshTools.Settings;

namespace SshTools.Line.Parameter.Keyword
{
    public abstract partial class Keyword : ISetting<string>
    {
        //TODO change IsHost to check if return type is of Type HostNode ...
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
        object ISetting.Key => Name;
        Type ISetting.Type => typeof(Keyword);
        string ISetting<string>.Key => Name;
    }
}