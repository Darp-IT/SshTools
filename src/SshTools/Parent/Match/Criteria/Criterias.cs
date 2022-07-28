using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace SshTools.Parent.Match.Criteria
{
    public partial class Criteria
    {
        /// <summary>
        /// Matches everytime (only allowed alone or directly after <see cref="Canonical"/> or <see cref="Final"/>)
        /// </summary>
        public static readonly SingleCriteria All = new SingleCriteria(nameof(All),
            (_, __) => true, criteria => Merged(criteria, CheckAll));

        public static readonly SingleCriteria Canonical = new SingleCriteria(nameof(Canonical),
            (_, __) => throw new NotImplementedException(), _ => Result.Ok());

        public static readonly SingleCriteria Final = new SingleCriteria(nameof(Final),
            (_, __) => throw new NotImplementedException(), _ => Result.Ok());

        /// <summary>
        /// The exec keyword executes the specified command under the user's shell.
        /// If the command returns a zero exit status then the condition is considered true.
        /// Commands containing whitespace characters must be quoted.
        /// Arguments to exec accept the tokens described in the TOKENS section.
        /// </summary>
        [Obsolete("Not sure if I want to support execution of user scripts right now")]
        public static readonly ArgumentCriteria Exec = new ArgumentCriteria(nameof(Exec),
            (_, __) => throw new NotImplementedException(),
            criteria => Merged(criteria, CheckNoAllBefore));

        /// <summary>
        /// Matches against the HostName, that is selected, when the Match is reached
        /// </summary>
        public static readonly ArgumentCriteria Host = new ArgumentCriteria(nameof(Host),
            (search, context) =>
                MatchingFunctions.MatchesKeyName<string>(nameof(MatchingContext.HostName), search, context),
            criteria => Merged(criteria, CheckNoAllBefore));

        /// <summary>
        /// Matches against the User, that is selected, when the Match is reached
        /// </summary>
        public static readonly ArgumentCriteria User = new ArgumentCriteria(nameof(User),
            (search, context) =>
                MatchingFunctions.MatchesKeyName<string>(nameof(MatchingContext.User), search, context),
            criteria => Merged(criteria, CheckNoAllBefore));

        /// <summary>
        /// Matches against the OriginalHost, as defined in the search
        /// </summary>
        public static readonly ArgumentCriteria OriginalHost = new ArgumentCriteria(nameof(OriginalHost),
            (search, context) =>
                MatchingFunctions.MatchesKeyName<string>(nameof(MatchingContext.OriginalHostName), search, context),
            criteria => Merged(criteria, CheckNoAllBefore));

        /// <summary>
        /// Matches against the LocalUser of the execution machine
        /// </summary>
        public static readonly ArgumentCriteria LocalUser = new ArgumentCriteria(nameof(LocalUser),
            (search, context) =>
                MatchingFunctions.MatchesKeyName<string>(nameof(MatchingContext.LocalUser), search, context),
            criteria => Merged(criteria, CheckNoAllBefore));

        public static Criteria[] Values => typeof(Criteria)
            .GetFields()
            .Select(f => f.GetValue(null))
            .Where(v => v is Criteria)
            .Cast<Criteria>()
            .ToArray();

        private static Result Merged(IList<Criteria> criteria, params CheckFunc[] checkFunc)
        {
            foreach (var func in checkFunc)
            {
                var res = func(criteria);
                if (res.IsFailed)
                    return res;
            }
            return Result.Ok();
        }
        
        private static Result CheckAll(IList<Criteria> criteria)
        {
            if (criteria.Count == 0) return Result.Ok();
            var lastCriteria = criteria[criteria.Count - 1];
            return lastCriteria == Canonical || lastCriteria == Final
                ? Result.Ok()
                : Result.Fail($"Criteria before {All} should be {Canonical} or {Final}");
        }

        private static Result CheckNoAllBefore(IEnumerable<Criteria> criteria)
        {
            return Result.FailIf(criteria.Any(c => c == All),
                $"Criteria {All} should be alone or after {Canonical} {Final}");
        }
    }
}