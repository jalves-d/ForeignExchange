using ForeignExchange.Application.DTOs;
using ForeignExchange.Domain.Entities;
using System.Threading.Tasks;

namespace ForeignExchange.Application.Interfaces
{
    public interface IExchangeRateService
    {
        Task<IEnumerable<ExchangeRate>> GetAllRatesAsync();
        Task<bool> AddRateAsync(ExchangeRateDTO rateDto);
        Task<bool> UpdateRateAsync(ExchangeRateDTO rateDto);
        Task<ExchangeRate?> GetLatestRateAsync(string currencyPair);
        Task<bool> DeleteRateAsync(string currencyPair);
        Task<ExchangeRate?> GetExchangeRateAsync(string currencyPair);
    }
}
