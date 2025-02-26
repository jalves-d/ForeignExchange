namespace ForeignExchange.Domain.Exceptions
{
    public class ForexProviderException : Exception
    {
        public ForexProviderException()
        {
        }

        public ForexProviderException(string message)
            : base(message)
        {
        }

        public ForexProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
