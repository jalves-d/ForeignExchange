using ForeignExchange.Application.Interfaces;
using ForeignExchange.Domain.Events;
using ForeignExchange.Infrastructure.Interfaces;
using System;

namespace ForeignExchange.Application.EventHandlers
{
    public class ExchangeRateUpdatedEventHandler : IEventHandler<ExchangeRateUpdatedEvent>
    {
        private readonly IForexProviderRepository _alphaVantageRepository;
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public ExchangeRateUpdatedEventHandler(IForexProviderRepository alphaVantageRepository, IExchangeRateRepository exchangeRateRepository)
        {
            _alphaVantageRepository = alphaVantageRepository;
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task HandleAsync(ExchangeRateUpdatedEvent @event)
        {
            var updatedRate = await _alphaVantageRepository.GetExchangeRateAsync(@event.CurrencyPair);

            if (updatedRate != null)
            {
                await _exchangeRateRepository.UpdateCurrencyPairAsync(updatedRate);
                Console.WriteLine($"[EventHandler] Updated exchange rate for {@event.CurrencyPair}");
            }
        }
    }

}
