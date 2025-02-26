using ForeignExchange.Domain.Entities;
using System.Threading.Tasks;

namespace ForeignExchange.Infrastructure.Interfaces
{
    public interface IExchangeRateRepository
    {
        Task<ExchangeRate?> GetByCurrencyPairAsync(string currencyPair);
        Task<ExchangeRate?> AddCurrencyPairAsync(ExchangeRate exchangeRate);
        Task<ExchangeRate?> UpdateCurrencyPairAsync(ExchangeRate exchangeRate);
        bool IsValidCurrencyPair(string currencyPair);
        Task<bool> DeleteCurrencyPairAsync(string currencyPair);
        Task<IEnumerable<ExchangeRate>> GetAllCurrencyPairsAsync();
    }
}
