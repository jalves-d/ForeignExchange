using ForeignExchange.Domain.Entities;
using ForeignExchange.Infrastructure.Interfaces;
using System.Threading.Tasks;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Domain.Events;
using ForeignExchange.Domain.Exceptions;
using ForeignExchange.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace ForeignExchange.Application.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly IMessageService _eventService;
        private readonly IForexProviderRepository _providerRepository;
        private readonly ILogger<ExchangeRateService> _logger;

        public ExchangeRateService(IExchangeRateRepository exchangeRateRepository, IForexProviderRepository providerRepository, IMessageService eventService, ILogger<ExchangeRateService> logger)
        {
            _exchangeRateRepository = exchangeRateRepository;
            _eventService = eventService;
            _providerRepository = providerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ExchangeRate>> GetAllRatesAsync()
        { 
            return await _exchangeRateRepository.GetAllCurrencyPairsAsync();
        }

        public async Task<bool> AddRateAsync(ExchangeRateDTO rateDto)
        {
            string currencyPair = $"{rateDto.BaseCurrency}-{rateDto.QuoteCurrency}";

            if (!_exchangeRateRepository.IsValidCurrencyPair(currencyPair))
            {
                throw new CurrencyPairException("Invalid currency format. Expected format: XXX");
            }

            var existingRate = await _exchangeRateRepository.GetByCurrencyPairAsync(currencyPair);

            if (existingRate == null) 
            {
                existingRate = new ExchangeRate();
                existingRate.CurrencyPair = currencyPair;
                existingRate.AskPrice = rateDto.AskPrice;
                existingRate.BidPrice = rateDto.BidPrice;
                existingRate.UpdatedAt = DateTime.UtcNow;
                await _exchangeRateRepository.AddCurrencyPairAsync(existingRate);
            }
            else
                throw new CurrencyPairException("The currency pair already exists!");

            return true;
        }

        public async Task<bool> UpdateRateAsync(ExchangeRateDTO rateDto)
        {
            string currencyPair = $"{rateDto.BaseCurrency}-{rateDto.QuoteCurrency}";

            if (!_exchangeRateRepository.IsValidCurrencyPair(currencyPair))
                throw new CurrencyPairException("Invalid currency format. Expected format: XXX");

            var existingRate = await _exchangeRateRepository.GetByCurrencyPairAsync(currencyPair);

            if (existingRate == null)
                throw new CurrencyPairException("The currency pair was not found!");

            existingRate.BidPrice = rateDto.BidPrice;
            existingRate.AskPrice = rateDto.AskPrice;
            existingRate.UpdatedAt = DateTime.UtcNow;

            await _exchangeRateRepository.UpdateCurrencyPairAsync(existingRate);

            return true;
        }

        public async Task<ExchangeRate?> GetLatestRateAsync(string currencyPair)
        {

            if (!_exchangeRateRepository.IsValidCurrencyPair(currencyPair))
                throw new CurrencyPairException("Invalid currency format. Expected format: XXX");
            
            try 
            { 
                var newRate = await _providerRepository.GetExchangeRateAsync(currencyPair);
                if (await _exchangeRateRepository.GetByCurrencyPairAsync(currencyPair) == null)   
                    await _exchangeRateRepository.AddCurrencyPairAsync(newRate);
                return newRate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving exchange rate for {CurrencyPair}", currencyPair);
                throw new CurrencyPairException(ex.Message);
            }

            return null;
        }

        public async Task<bool> DeleteRateAsync(string currencyPair)
        {
            if (!_exchangeRateRepository.IsValidCurrencyPair(currencyPair))
                throw new CurrencyPairException("Invalid currency format. Expected format: XXX");

            return await _exchangeRateRepository.DeleteCurrencyPairAsync(currencyPair);
        }

        public async Task<ExchangeRate?> GetExchangeRateAsync(string currencyPair)
        {
            if (!_exchangeRateRepository.IsValidCurrencyPair(currencyPair))
                throw new CurrencyPairException("Invalid currency pair. Both 'fromCurrency' and 'toCurrency' must be provided. Valid format example: EUR-BRL");

            var exchangeRate = await _exchangeRateRepository.GetByCurrencyPairAsync(currencyPair);
            
            //To activate eventService comment the code bellow
            if (exchangeRate != null)
                return exchangeRate;
            //-----------------------|
        
            //To activate eventService use the code bellow
            /*if (exchangeRate != null)
            {
                await _eventService.PublishAsync(new ExchangeRateUpdatedEvent(currencyPair));
            }
            else
            {*/
                try
                { 
                    exchangeRate = _providerRepository.GetExchangeRateAsync(currencyPair).Result;
                    await _exchangeRateRepository.AddCurrencyPairAsync(exchangeRate);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while retrieving exchange rate for {CurrencyPair}", currencyPair);
                    throw new CurrencyPairException(ex.Message);
                }               
            //}

            return exchangeRate;
        }
    }
}
