namespace ForeignExchange.Domain.Events
{
    public class ExchangeRateUpdatedEvent
    {
        public string CurrencyPair { get; }

        public ExchangeRateUpdatedEvent(string currencyPair)
        {
            if (string.IsNullOrWhiteSpace(currencyPair))
                throw new ArgumentException("Currency pair cannot be null or empty.", nameof(currencyPair));

            CurrencyPair = currencyPair;
        }

        public override string ToString()
        {
            return $"{nameof(ExchangeRateUpdatedEvent)}: {CurrencyPair}";
        }
    }
}
