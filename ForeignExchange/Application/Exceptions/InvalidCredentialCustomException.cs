using System;

namespace ForeignExchange.Application.Exceptions
{
    public class InvalidCredentialCustomException : Exception
    {
        public InvalidCredentialCustomException()
        {
        }

        public InvalidCredentialCustomException(string message)
            : base(message)
        {
        }

        public InvalidCredentialCustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
