using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentResults;
using SshTools.Config.Extensions;
using SshTools.Config.Matching;
using SshTools.Config.Parameters;
using SshTools.Config.Util;

namespace SshTools.Config.Parents
{
    public static class LineListExtensions
    {
        //-----------------------------------------------------------------------//
        //                      Basic Functions for Parameters
        //-----------------------------------------------------------------------//

        /// <summary>
        /// Checks if given sequence contains an entry with the given keyword
        /// </summary>
        /// <param name="lines">A sequence of lines to check on</param>
        /// <param name="keyword">The <see cref="Keyword"/> to be searched for</param>
        /// <returns>Whether the sequence contains an entry with this keyword</returns>
        public static bool Has(this IEnumerable<ILine> lines, Keyword keyword) => 
            lines.Any(p => p.Is(keyword));
        
        /// <summary>
        /// Returns the argument of the first matching element of the given <paramref name="lines"/>
        /// </summary>
        /// <param name="lines">A sequence of lines to get from</param>
        /// <param name="keyword">The <see cref="Keyword{T}"/> to be searched for</param>
        /// <typeparam name="TParam">Argument type</typeparam>
        /// <returns>First matching Argument as <typeparamref name="TParam"/> or default</returns>
        public static TParam Get<TParam>(this IEnumerable<ILine> lines, Keyword<TParam> keyword) => 
            (TParam) lines.Get((Keyword) keyword);

        /// <summary>
        /// Non generic way to get the argument by key as an object from the given sequence.
        /// Use only when necessary
        /// </summary>
        /// <param name="lines">A sequence of lines to get from</param>
        /// <param name="keyword">The <see cref="Keyword"/> to be searched for</param>
        /// <returns>First argument as <see cref="object"/> or default</returns>
        internal static object Get(this IEnumerable<ILine> lines, Keyword keyword) =>
            lines
                .OfParameter()
                .Where(p => p.Is(keyword))
                .Select(p => p.Argument)
                .FirstOrDefault() ?? keyword.GetDefault();
        
        /// <summary>
        /// Returns the index of the given <paramref name="keyword"/>
        /// If the given sequence does not contain it the return will be -1
        /// </summary>
        /// <param name="lines">A sequence of lines to be searched</param>
        /// <param name="keyword">The <see cref="Keyword"/> to be searched for</param>
        /// <returns>Index of the element, -1 if not available</returns>
        public static int IndexOf(this IList<ILine> lines, Keyword keyword)
        {
            lines.ThrowIfNull();
            keyword.ThrowIfNull();
            for (var i = 0; i < lines.Count; i++)
            {
                if (lines[i] is IParameter param && param.Keyword.Equals(keyword))
                    return i;
            }
            return -1;
        }
        
        /// <summary>
        /// Inserts a given value <typeparamref name="TValue"/> into the sequence of lines at <paramref name="index"/>. 
        /// Dependent on <paramref name="ignoreCount"/> the method will check for an already existing one.<br/>
        /// If the <paramref name="index"/> is negative the insertion will be counted from the back
        /// </summary>
        /// <param name="lines">A sequence of lines to be inserted to</param>
        /// <param name="index">The index to be inserted at in range [ -(Count+1); Count-1 ]</param>
        /// <param name="keyword">The keyword to insert</param>
        /// <param name="value">The value to be inserted</param>
        /// <param name="ignoreCount">Whether the insertion will be executed if a keyword, that is only allowed once,
        /// is already present</param>
        /// <typeparam name="TValue">The type of <paramref name="value"/></typeparam>
        /// <returns><see cref="Result{TValue}"/> with optionally value or reason for failure</returns>
        public static Result<TValue> Insert<TValue>(this IList<ILine> lines,
            int index, Keyword<TValue> keyword, TValue value, bool ignoreCount = false)
        {
            lines.ThrowIfNull();
            keyword.ThrowIfNull();
            value.ThrowIfNull();
            if (index < -lines.Count-1 || index > lines.Count) Result.Fail(
                $"Index is out of bounds! May be in range of {-lines.Count-1};{lines.Count} but is {index}");
            if (!ignoreCount && !keyword.AllowMultiple && lines.Has(keyword))
                return Result.Fail<TValue>($"Already containing entry with keyword {keyword}");
            var insertionRes = Result.Try(() => 
                lines.Insert(
                    index >= 0 ? index : lines.Count + index + 1, 
                    new Parameter<TValue>(keyword, value, ParameterAppearance.Default(keyword))));
            return insertionRes.IsSuccess
                ? Result.Ok(value)
                : insertionRes.ToResult<TValue>();
        }
        
        /// <summary>
        /// Sets the given <paramref name="value"/> to the sequence of <paramref name="lines"/>
        /// </summary>
        /// <param name="lines">A sequence of lines to set to</param>
        /// <param name="keyword">The <see cref="Keyword{T}"/> to be set</param>
        /// <param name="value">The value to be set</param>
        /// <typeparam name="TParam">Type of the value</typeparam>
        /// <returns><see cref="Result{TValue}"/> with optionally value or reason for failure</returns>
        public static Result<TParam> Set<TParam>(this IList<ILine> lines, Keyword<TParam> keyword, TParam value)
        {
            lines.ThrowIfNull();
            keyword.ThrowIfNull();
            value.ThrowIfNull();
            if (!lines.Has(keyword)) 
                return lines.Insert(0, keyword, value, true);
            var param = lines.OfParameter().First(p => p.Is(keyword));
            param.Argument = value;
            return Result.Ok(value);
        }

        /// <summary>
        /// Removes <paramref name="maxCount"/> of lines, that fulfill the <paramref name="func"/>
        /// </summary>
        /// <param name="lines">The sequence of lines to be removed from</param>
        /// <param name="func">The predicate to be matched</param>
        /// <param name="maxCount">The maximum of lines to be removed</param>
        /// <returns>The number of removed lines</returns>
        public static int Remove(this IList<ILine> lines, Func<ILine, bool> func, int maxCount = int.MaxValue)
        {
            lines.ThrowIfNull();
            func.ThrowIfNull();
            var i = 0;
            var count = 0;
            while (i < lines.Count && count < maxCount)
            {
                if (func(lines[i]))
                {
                    lines.RemoveAt(i);
                    count++;
                }
                else
                {
                    i++;
                }
            }
            return count;
        }

        /// <summary>
        /// Removes the first <paramref name="maxCount"/> matching entries in the given sequence
        /// </summary>
        /// <param name="lines">A sequence of lines to be removed from</param>
        /// <param name="keyword">The <see cref="Keyword"/> to be removed</param>
        /// <param name="maxCount">Maximum count of items to be removed. Default is only 1!</param>
        /// <returns>Number of removed lines with matching keyword</returns>
        public static int Remove(this IList<ILine> lines, Keyword keyword, int maxCount = 1) =>
            lines.Remove(l => l.Is(keyword), maxCount);
        
        /// <summary>
        /// Removes <paramref name="maxCount"/> of <paramref name="lines"/> that extend <typeparamref name="T"/> and fulfill the given <paramref name="func"/>
        /// </summary>
        /// <param name="lines">The sequence of lines to be removed from</param>
        /// <param name="func">The predicate to be matched</param>
        /// <param name="maxCount">The maximum of lines to be removed</param>
        /// <typeparam name="T">The type that the parameter's arguments should extend from</typeparam>
        /// <returns>The number of removed lines</returns>
        public static int Remove<T>(this IList<ILine> lines, Func<T, bool> func, int maxCount = int.MaxValue) =>
            lines.Remove(line => line is IArgParameter<T> param && func(param.Argument), maxCount);
        
        //-----------------------------------------------------------------------//
        //                      Basic Functions for ParameterParents
        //-----------------------------------------------------------------------//
        
        /// <summary>
        /// Checks whether a node of <paramref name="name"/> is present int he given sequence
        /// </summary>
        /// <param name="lines">The sequence of <paramref name="lines"/> to be checked</param>
        /// <param name="name">The name of the node to be looked for</param>
        /// <param name="options">Options how the lookup should be performed</param>
        /// <returns>whether the <paramref name="name"/> is contained in <paramref name="lines"/></returns>
        public static bool Has(this IEnumerable<ILine> lines, string name, MatchingOptions options = MatchingOptions.EXACT) =>
            lines.Matching(name, options).WhereArg<Node>().Any();

        /// <summary>
        /// Returns the index of the given <paramref name="hostName"/>
        /// If the given sequence does not contain it the return will be -1
        /// </summary>
        /// <param name="lines">A sequence of lines to be searched</param>
        /// <param name="hostName">The name of the host to be searched for</param>
        /// <returns>Index of the element, -1 if not available</returns>
        public static int IndexOf(this IList<ILine> lines, string hostName)
        {
            for (var i = 0; i < lines.Count; i++)
            {
                if (!(lines[i] is IParameter param)) continue;
                if (param.Argument is Node node && node.Name.Equals(hostName))
                    return i;
            }
            return -1;
        }
        
        /// <summary>
        /// Gets a reference to the first matching <see cref="HostNode"/> in the given sequence.
        /// </summary>
        /// <param name="lines">The sequence of <paramref name="lines"/> to be searched</param>
        /// <param name="hostName">The name of the Host to be looked for</param>
        /// <returns>The first <see cref="HostNode"/> or null of nothing could be found</returns>
        /// <seealso cref="Find"/>
        public static HostNode Get(this IEnumerable<ILine> lines, string hostName) => lines
                .Matching(hostName, MatchingOptions.EXACT)
                .WhereArg<HostNode>()
                .SelectArg()
                .FirstOrDefault();
        
        /// <summary>
        /// Private helper method to find a match keyword and insert a match at a specific position
        /// </summary>
        private static Result<T> InsertNode<T>(this IList<ILine> lines, int index, T node)
            where T : Node
        {
            var res = SshTools.Settings.GetKeyword<T>();
            return res.IsSuccess
                ? lines.Insert(index, res.Value, node)
                : res.ToResult<T>();
        }

        /// <summary>
        /// Inserts a new <see cref="HostNode"/> of given <paramref name="hostName"/> at <paramref name="index"/>
        /// </summary>
        /// <param name="lines">The list of lines to be set to</param>
        /// <param name="index">The index to be inserted at in range [ -(Count+1); Count-1 ]</param>
        /// <param name="hostName">The name of the new Host</param>
        /// <returns>A <see cref="Result{TValue}"/> with optionally the inserted Host</returns>
        public static Result<HostNode> InsertHost(this IList<ILine> lines, int index, string hostName)
        {
            return string.IsNullOrWhiteSpace(hostName)
                ? Result.Fail<HostNode>("The hostName must not be null or only whitespace")
                : lines.InsertNode(index, new HostNode(hostName));
        }

        /// <summary>
        /// Inserts a new <see cref="MatchNode"/> at <paramref name="index"/> with a new <see cref="Criteria"/>
        /// </summary>
        /// <param name="lines">The list of lines to be inserted at</param>
        /// <param name="index">The index to be inserted at in range [ -(Count+1); Count-1 ]</param>
        /// <param name="criteria">The criteria of the match</param>
        /// <returns>A <see cref="Result{TValue}"/> with optionally the inserted Match</returns>
        public static Result<MatchNode> InsertMatch(this IList<ILine> lines, int index, Criteria criteria)
        {
            var match = new MatchNode();
            var res = match.Set(criteria);
            return res.IsFailed
                ? res.ToResult<MatchNode>()
                : lines.InsertNode(index, match);
        }
        
        /// <summary>
        /// Inserts a new <see cref="MatchNode"/> at <paramref name="index"/> with a new <see cref="Criteria"/>
        /// </summary>
        /// <param name="lines">The list of lines to be inserted at</param>
        /// <param name="index">The index to be inserted at in range [ -(Count+1); Count-1 ]</param>
        /// <param name="criteria">The criteria of the match</param>
        /// <param name="argument">The argument of the <paramref name="criteria"/></param>
        /// <returns>A <see cref="Result{TValue}"/> with optionally the inserted Match</returns>
        public static Result<MatchNode> InsertMatch(this IList<ILine> lines, int index,
            ArgumentCriteria criteria, params string[] argument)
        {
            var match = new MatchNode();
            var res = match.Set(criteria, argument);
            return res.IsFailed
                ? res.ToResult<MatchNode>()
                : lines.InsertNode(index, match);
        }
        
        /// <summary>
        /// Private helper method to find a match keyword and insert a match at a specific position
        /// </summary>
        private static Result<T> SetNode<T>(this IList<ILine> lines, T node)
            where T : Node
        {
            var res = SshTools.Settings.GetKeyword<T>();
            if (res.IsFailed) return res.ToResult<T>();
            var matchString = node.Name;
            var param = lines
                .WhereParam(res.Value)
                .FirstOrDefault(p => p.Argument.Name.Equals(matchString));
            if (param is null)
                return lines.Insert(0, res.Value, node, true);
            param.Argument = node;
            return Result.Ok(node);
        }
        
        /// <summary>
        /// Executes one of the following:
        /// <list type="bullet">
        /// <item>Replaces the first defined <see cref="HostNode"/> of matching <paramref name="hostName"/>
        /// with an empty host, if there is already an entry defined</item>
        /// <item>Inserts a new <see cref="HostNode"/> at the beginning otherwise</item>
        /// </list>
        /// </summary>
        /// <param name="lines">The list of lines to be set to</param>
        /// <param name="hostName">The name of the host to be set</param>
        /// <returns>A <see cref="Result{TValue}"/> with optionally the set host</returns>
        public static Result<HostNode> SetHost(this IList<ILine> lines, string hostName) =>
            lines.SetNode(new HostNode(hostName));
        
        
        /// <summary>
        /// Executes one of the following:
        /// <list type="bullet">
        /// <item>Replaces the first defined <see cref="MatchNode"/> of matching <paramref name="criteria"/>
        /// with an empty host, if there is already an entry defined</item>
        /// <item>Inserts a new <see cref="MatchNode"/> at the beginning otherwise</item>
        /// </list>
        /// </summary>
        /// <param name="lines">The list of lines to be set to</param>
        /// <param name="criteria">The criteria of the match to be set</param>
        /// <returns>A <see cref="Result{TValue}"/> with optionally the set match</returns>
        public static Result<MatchNode> SetMatch(this IList<ILine> lines, Criteria criteria)
        {
            var match = new MatchNode();
            match.Set(criteria);
            return lines.SetNode(match);
        }

        /// <summary>
        /// Executes one of the following:
        /// <list type="bullet">
        /// <item>Replaces the first defined <see cref="MatchNode"/> of matching <paramref name="criteria"/>
        /// with an empty host, if there is already an entry defined</item>
        /// <item>Inserts a new <see cref="MatchNode"/> at the beginning otherwise</item>
        /// </list>
        /// </summary>
        /// <param name="lines">The list of lines to be set to</param>
        /// <param name="criteria">The criteria of the match to be set</param>
        /// <param name="argument">The argument of the <paramref name="criteria"/></param>
        /// <returns>A <see cref="Result{TValue}"/> with optionally the set match</returns>
        public static Result<MatchNode> SetMatch(this IList<ILine> lines, ArgumentCriteria criteria, string argument)
        {
            var match = new MatchNode();
            match.Set(criteria, argument);
            return lines.SetNode(match);
        }

        /// <summary>
        /// Removes <paramref name="maxCount"/> of nodes, that are matching the given <paramref name="name"/>
        /// </summary>
        /// <param name="lines">The sequence of lines to be removed from</param>
        /// <param name="name">The name of the node to be matched against</param>
        /// <param name="maxCount">The maximum of lines to be removed</param>
        /// <param name="options">Options, that specify the comparison</param>
        /// <returns>The number of removed lines</returns>
        public static int Remove(this IList<ILine> lines, string name,
            int maxCount = int.MaxValue,
            MatchingOptions options = MatchingOptions.EXACT) =>
            lines.Remove<Node>(param => param.Matches(name, new MatchingContext(name), options), maxCount);
        
        
        //-----------------------------------------------------------------------//
        //                    LINQ like Functions for Parameters
        //-----------------------------------------------------------------------//
        
        /// <summary>
        /// Selects all lines of type <see cref="Node"/>
        /// </summary>
        /// <param name="lines">The sequence of lines to be selected from</param>
        /// <returns>A sequence of type <see cref="Node"/></returns>
        public static IEnumerable<Node> Nodes(this IEnumerable<ILine> lines) => lines
            .WhereArg<Node>()
            .SelectArg();
        
        /// <summary>
        /// Selects all lines of type <see cref="HostNode"/>
        /// </summary>
        /// <param name="lines">The sequence of lines to be selected from</param>
        /// <returns>A sequence of type <see cref="HostNode"/></returns>
        public static IEnumerable<HostNode> Hosts(this IEnumerable<ILine> lines) => lines
            .WhereArg<HostNode>()
            .SelectArg();
        
        /// <summary>
        /// Selects all lines of type <see cref="MatchNode"/>
        /// </summary>
        /// <param name="lines">The sequence of lines to be selected from</param>
        /// <returns>A sequence of type <see cref="MatchNode"/></returns>
        public static IEnumerable<MatchNode> Matches(this IEnumerable<ILine> lines) => lines
            .WhereArg<MatchNode>()
            .SelectArg();

        /// <summary>
        /// Flattens the given sequence <paramref name="lines"/>.
        /// Generates a list of all <paramref name="lines"/>, contained in <paramref name="lines"/>;
        /// Creates new and empty nodes and automatically resolves includes
        /// </summary>
        /// <param name="lines">A sequence of lines to flatten</param>
        /// <param name="resolveIncludes">Specifies if the flatten will be recursively used for underlying configs</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of the flattening</returns>
        public static IEnumerable<ILine> Flatten(this IEnumerable<ILine> lines, bool resolveIncludes = true)
        {
            lines.ThrowIfNull();
            var afterFirstHost = false;
            foreach (var line in lines)
            {
                if (!(line is IParameter param))
                {
                    yield return line;
                    continue;
                }
                foreach (var comment in param.Comments.Comments)
                {
                    yield return comment;
                }
                if (param.Argument is IEnumerable<ILine> includeParams)
                {
                    afterFirstHost = true;
                    if (param is IParameter<HostNode> hostParam)
                        yield return new Parameter<HostNode>(
                            hostParam.Keyword,
                            (HostNode) hostParam.Argument.Copy(),
                            hostParam.ParameterAppearance);
                    else if (param is IParameter<MatchNode> matchParam)
                        yield return new Parameter<MatchNode>(
                            matchParam.Keyword,
                            (MatchNode) matchParam.Argument.Copy(),
                            matchParam.ParameterAppearance);
                    var enumerable = resolveIncludes
                        ? includeParams.Flatten()
                        : includeParams;
                    foreach (var includeParameters in enumerable)
                        yield return includeParameters;
                }
                else
                {
                    if (!afterFirstHost)
                       yield return line;
                }
            }
        }

        /// <summary>
        /// Collects all <paramref name="lines"/> and groups them in hosts
        /// </summary>
        /// <param name="lines">A sequence of lines to be collected from</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of collecting</returns>
        public static IEnumerable<ILine> Collect(this IEnumerable<ILine> lines)
        {
            lines.ThrowIfNull();
            IArgParameter<ParameterParent> lastNode = null;
            IList<Comment> comments = new List<Comment>();
            foreach (var line in lines)
            {
                switch (line)
                {
                    case Comment comment:
                        comments.Add(comment);
                        break;
                    case IParameter param:
                    {
                        param.Comments.AddRange(comments);
                        comments.Clear();
                        if (line.IsNode() && line is IArgParameter<ParameterParent> parent)
                        {
                            if (lastNode != null)
                                yield return lastNode;
                            lastNode = parent;
                        }
                        else
                        {
                            if (lastNode != null) 
                                lastNode.Argument.Add(line);
                            else
                                yield return line;
                        }

                        break;
                    }
                }
            }
            if (lastNode != null)
                yield return lastNode;
            foreach (var comment in comments)
            {
                yield return comment;
            }
        }

        /// <summary>
        /// Clones all <paramref name="lines"/> of given <paramref name="lines"/> <br/>
        /// Cloned configs will loose all information about comments on the tail of the config
        /// </summary>
        /// <param name="lines">A sequence of lines to invoke the clone on.</param>
        /// <typeparam name="TLine">The type of the given <paramref name="lines" /></typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the clone function
        /// on each parameter of <paramref name="lines" /></returns>
        public static IEnumerable<TLine> Cloned<TLine>(this IEnumerable<TLine> lines)
            where TLine : ILine =>
            lines.Select(parameter => (TLine) parameter.Clone());

        /// <summary>
        /// Compiles the given sequence by applying
        /// <list type="bullet">
        ///<item><see cref="Flatten"/></item>
        ///<item><see cref="Collect"/></item>
        ///<item><see cref="Cloned{TParam}"/></item>
        /// </list>
        /// </summary>
        /// <param name="lines">A sequence of ILine to be compiled</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of the compilation</returns>
        public static IEnumerable<ILine> Compiled(this IEnumerable<ILine> lines) =>
            lines.Flatten()
                .Collect()
                .Cloned();
        
        /// <summary>
        /// Scans every parameter in the given sequence and only returns matching ones <br/>
        /// If a line is a comment it will be skipped, normal parameters as well as all
        /// <see cref="Node"/>s, that apply to <see cref="Node.Matches"/> are being yielded
        /// </summary>
        /// <param name="lines">A sequence of ILine to be matched</param>
        /// <param name="name">The name to be matched against</param>
        /// <param name="options">The options for the matching</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are matching the <paramref name="name"/></returns>
        public static IEnumerable<IParameter> Matching(
            this IEnumerable<ILine> lines,
            string name,
            MatchingOptions options = MatchingOptions.PATTERN)
        {
            lines.ThrowIfNull();
            name.ThrowIfNull();
            options.ThrowIfNull();
            var context = new MatchingContext(name);
            foreach (var line in lines)
            {
                if (!(line is IParameter param)) continue;
                if (param.Argument is Node node)
                {
                    if (node.Matches(name, context, options))
                        yield return param;
                }
                else
                {
                    context.SetProperty(param.Argument, param.Keyword.Name);
                    yield return param;
                }
            }
        }

        /// <summary>
        /// Filters the given sequence, where the parameter type is exactly of <typeparamref name="TParam"/>
        /// </summary>
        /// <param name="lines">A sequence of lines to be filtered</param>
        /// <param name="keyword">The <see cref="Keyword{TParam}"/> to be searched for</param>
        /// <typeparam name="TParam">The type to be filtered for</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are of type <typeparamref name="TParam"/></returns>
        public static IEnumerable<IParameter<TParam>> WhereParam<TParam>(
            this IEnumerable<ILine> lines,
            Keyword<TParam> keyword)
        {
            lines.ThrowIfNull();
            keyword.ThrowIfNull();
            foreach (var item in lines)
                    if (item is IParameter<TParam> param && param.Keyword == keyword)
                        yield return param;
            
        }
        
        /// <summary>
        /// Filters the given sequence, where the argument's type is extending from <typeparamref name="TParam"/>
        /// </summary>
        /// <param name="lines">A sequence of lines to be filtered</param>
        /// <typeparam name="TParam">The type the arguments should extend from</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are of type <typeparamref name="TParam"/></returns>
        public static IEnumerable<IArgParameter<TParam>> WhereArg<TParam>(this IEnumerable<ILine> lines)
        {
            lines.ThrowIfNull();
            foreach (var item in lines)
                if (item is IArgParameter<TParam> param)
                    yield return param;
        }
        
        /// <summary>
        /// Selects the arguments of all lines, that are of type <see cref="IParameter"/> and ignores comments
        /// </summary>
        /// <param name="lines">A sequence of lines to be selected</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the selected arguments</returns>
        public static IEnumerable<object> SelectArg(this IEnumerable<ILine> lines) => 
            lines.OfParameter().Select(p => p.Argument);

        /// <summary>
        /// Selects all <paramref name="lines"/>, that are of type <see cref="IParameter"/>
        /// </summary>
        /// <param name="lines">A sequence of lines to be selected from</param>
        /// <returns>A list containing all parameters of the gives <paramref name="lines"/></returns>
        public static IEnumerable<IParameter> OfParameter(this IEnumerable<ILine> lines) => 
            lines.OfType<IParameter>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines">A sequence of lines to be selected</param>
        /// <typeparam name="TParam">The type of arguments to be selected</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the selected arguments
        /// of type <typeparamref name="TParam"/></returns>
        public static IEnumerable<TParam> SelectArg<TParam>(this IEnumerable<IArgParameter<TParam>> lines) =>
            lines.Select(p => p.Argument);

        //-----------------------------------------------------------------------//
        //                           Consumer functions
        //-----------------------------------------------------------------------//

        /// <summary>
        /// Consumes given enumerable of <paramref name="lines"/> and returns a new <see cref="SshConfig"/>
        /// </summary>
        /// <param name="lines">A sequence of lines to create the config of</param>
        /// <param name="fileName">Optional file name, as initializer for the SshConfig</param>
        /// <returns>SshConfig</returns>
        public static SshConfig ToConfig(this IEnumerable<ILine> lines, string fileName = null)
        {
            lines.ThrowIfNull();
            return new SshConfig(fileName, lines.Collect().ToList());
        }

        /// <summary>
        /// Consumes given lines and returns all parameters in a new <see cref="HostNode"/>.
        /// If a parameter is defined multiple times (except from those intended) only the first will be there.<br/>
        /// If a line contains a <see cref="Node"/> this line and all parameters inside the Node will be ignored.<br/>
        /// The newly created host will contain the following comments:
        /// <list type="bullet">
        /// <item>If the given <paramref name="lines"/> are of type <see cref="Node"/> all their comments will be added</item>
        /// <item>If the line is a normal parameter all their comments will be added</item>
        /// <item>If the line is a <see cref="Comment"/> it will be added</item>
        /// </list>
        /// All comments are only being added if they are not empty (or consist of only whitespaces)<br/>
        /// Use <see cref="Flatten"/> to create a host out of all parameters
        /// </summary>
        /// <param name="lines">A sequence of lines to create the host of</param>
        /// <param name="hostName">HostName of the Host, used as initializer</param>
        /// <returns>HostNode</returns>
        public static HostNode ToHost(this IEnumerable<ILine> lines, string hostName)
        {
            lines.ThrowIfNull();
            hostName.ThrowIfNull();
            var host = new HostNode(hostName);
            if (lines is Node parent)
                foreach (var parentComment in parent.Comments)
                    if (!string.IsNullOrWhiteSpace(parentComment)) 
                        host.Comments.Add(parentComment);

            foreach (var line in lines)
            {
                switch (line)
                {
                    case IArgParameter<Node> _:
                        continue;
                    case IParameter param when param.Keyword.AllowMultiple || !host.Has(param.Keyword):
                    {
                        host.Add(line);
                        foreach (var parentComment in param.Comments.Comments)
                            if (!string.IsNullOrWhiteSpace(parentComment.Argument))
                                host.Comments.Add(parentComment);
                        break;
                    }
                    case Comment comment:
                        if (!string.IsNullOrWhiteSpace(comment.Argument)) 
                            host.Comments.Add(comment);
                        break;
                }
            }
            return host;
        }
        
        /// <summary>
        /// Compiles all matching <paramref name="lines"/> of a config into a new host.
        /// The host will be completely uncoupled from the config.<br/>
        /// If the list of <paramref name="lines"/> is empty or no matching host could be found,
        /// the resulting <see cref="HostNode"/> will be empty too
        /// </summary>
        /// <param name="lines">A sequence of lines to be searched</param>
        /// <param name="hostName">The name of the Host to be searched for</param>
        /// <param name="options">Matching options for the <see cref="hostName"/></param>
        /// <returns>The new Host</returns>
        public static HostNode Find(this IEnumerable<ILine> lines,
            string hostName,
            MatchingOptions options = MatchingOptions.PATTERN) =>
            lines
                .Compiled()
                .Matching(hostName, options)
                .Flatten(false)
                .ToHost(hostName);
        
        /// <summary>
        /// Creates a new SshConfig, with all includes
        /// </summary>
        /// <param name="lines">The sequence of lines to be compiled</param>
        /// <returns>New SshConfig</returns>
        public static SshConfig Compile(this IEnumerable<ILine> lines) =>
            lines.Compiled().ToConfig();

        /// <summary>
        /// Queries the given sequence of lines to select those of type <see cref="Node"/> and bakes them into a new list
        /// </summary>
        /// <param name="lines">The sequence to be selected</param>
        /// <returns>A list of type <see cref="Node"/></returns>
        public static IList<Node> GetNodes(this IEnumerable<ILine> lines) => lines.Nodes().ToList();
        
        /// <summary>
        /// Queries the given sequence of lines to select those of type <see cref="Node"/> and bakes them into a new list
        /// </summary>
        /// <param name="lines">The sequence to be selected</param>
        /// <returns>A list of type <see cref="HostNode"/></returns>
        public static IList<HostNode> GetHosts(this IEnumerable<ILine> lines) => lines.Hosts().ToList();
        
        /// <summary>
        /// Queries the given sequence of lines to select those of type <see cref="MatchNode"/> and bakes them into a new list
        /// </summary>
        /// <param name="lines">The sequence to be selected</param>
        /// <returns>A list of type <see cref="MatchNode"/></returns>
        public static IList<MatchNode> GetMatches(this IEnumerable<ILine> lines) => lines.Matches().ToList();
        
        /// <summary>
        /// Consumes the given sequence of lines and serializes them into a string.
        /// By changing the options one can specify the look
        /// </summary>
        /// <param name="lines">A sequence of lines to be serialized</param>
        /// <param name="options">The options for exporting</param>
        /// <returns>Serialized string</returns>
        public static string Serialize(
            this IEnumerable<ILine> lines,
            SerializeConfigOptions options = SerializeConfigOptions.DEFAULT)
        {
            lines.ThrowIfNull();
            if (lines is IConfigSerializable serializable)
                return serializable.Serialize(options);
            var serializedList = new List<string>();
            serializedList.AddRange(lines.Select(p => p.Serialize(options)));
            return string.Join(Environment.NewLine, serializedList);
        }
        
        /// <summary>
        /// Writes a file to the path specified with <paramref name="filename"/>
        /// </summary>
        /// <param name="lines">A sequence of lines to be written</param>
        /// <param name="filename">The path to the file</param>
        /// <returns>Result if export was successful</returns>
        public static Result WriteFile(this IEnumerable<ILine> lines, string filename)
        {
            lines.ThrowIfNull();
            filename.ThrowIfNull();
            return Result.Try(() => File.WriteAllText(filename, lines.Serialize()));
        }
    }
}