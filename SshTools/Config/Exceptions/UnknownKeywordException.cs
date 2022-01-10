using System;
using FluentResults;

namespace SshTools.Config.Exceptions
{
    public class ResultException : Exception
    {
        public ResultException(ResultBase result) : base(string.Join(", ", result.Reasons))
        {
            
        }
    }
}