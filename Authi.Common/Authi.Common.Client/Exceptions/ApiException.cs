using System;

namespace Authi.Common.Client.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
    }
}
