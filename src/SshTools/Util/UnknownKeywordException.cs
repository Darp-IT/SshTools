using System;
using FluentResults;

namespace SshTools.Util
{
    public class ResultException : Exception
    {
        public ResultException(ResultBase result) : base(string.Join(", ", result.Reasons))
        {
            
        }
    }
}