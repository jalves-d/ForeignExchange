namespace ForeignExchange.Domain.Exceptions
{
    public class CurrencyPairException : Exception
    {
        public CurrencyPairException()
        {
        }

        public CurrencyPairException(string message)
            : base(message)
        {
        }

        public CurrencyPairException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
