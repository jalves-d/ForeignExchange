using ForeignExchange.Domain.Entities;

namespace ForeignExchange.Infrastructure.Interfaces
{
    public interface IForexProviderRepository
    {
        Task<ExchangeRate?> GetExchangeRateAsync(string currencyPair);
    }

}
