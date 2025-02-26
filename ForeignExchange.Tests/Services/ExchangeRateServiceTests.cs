using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Domain.Entities;
using ForeignExchange.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using ForeignExchange.Application.Services;
using ForeignExchange.Infrastructure.Interfaces;

namespace ForeignExchange.Tests.Services
{
    public class ExchangeRateServiceTests
    {
        private readonly Mock<IExchangeRateRepository> _exchangeRateRepositoryMock;
        private readonly Mock<IMessageService> _eventServiceMock;
        private readonly Mock<IForexProviderRepository> _providerRepositoryMock;
        private readonly Mock<ILogger<ExchangeRateService>> _loggerMock;
        private readonly ExchangeRateService _exchangeRateService;

        public ExchangeRateServiceTests()
        {
            _exchangeRateRepositoryMock = new Mock<IExchangeRateRepository>();
            _eventServiceMock = new Mock<IMessageService>();
            _providerRepositoryMock = new Mock<IForexProviderRepository>();
            _loggerMock = new Mock<ILogger<ExchangeRateService>>();
            _exchangeRateService = new ExchangeRateService(
                _exchangeRateRepositoryMock.Object,
                _providerRepositoryMock.Object,
                _eventServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllRatesAsync_ShouldReturnAllRates()
        {
            // Arrange
            var exchangeRates = new List<ExchangeRate>
            {
                new ExchangeRate { CurrencyPair = "USD-EUR", AskPrice = 1.1m, BidPrice = 1.0m },
                new ExchangeRate { CurrencyPair = "EUR-GBP", AskPrice = 0.9m, BidPrice = 0.8m }
            };
            _exchangeRateRepositoryMock.Setup(r => r.GetAllCurrencyPairsAsync()).ReturnsAsync(exchangeRates);

            // Act
            var result = await _exchangeRateService.GetAllRatesAsync();

            // Assert
            result.Should().BeEquivalentTo(exchangeRates);
        }

        [Fact]
        public async Task AddRateAsync_ShouldThrowException_WhenCurrencyPairIsInvalid()
        {
            // Arrange
            var rateDto = new ExchangeRateDTO { BaseCurrency = "INVALID", QuoteCurrency = "USD", AskPrice = 1.2m, BidPrice = 1.1m };
            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(false);

            // Act
            Func<Task> act = async () => await _exchangeRateService.AddRateAsync(rateDto);

            // Assert
            await act.Should().ThrowAsync<CurrencyPairException>()
                .WithMessage("Invalid currency format. Expected format: XXX");
        }

        [Fact]
        public async Task AddRateAsync_ShouldAddCurrencyPair_WhenValid()
        {
            // Arrange
            var rateDto = new ExchangeRateDTO { BaseCurrency = "USD", QuoteCurrency = "EUR", AskPrice = 1.2m, BidPrice = 1.1m };
            var existingRate = (ExchangeRate)null;

            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(true);
            _exchangeRateRepositoryMock.Setup(r => r.GetByCurrencyPairAsync("USD-EUR")).ReturnsAsync(existingRate);

            // Act
            var result = await _exchangeRateService.AddRateAsync(rateDto);

            // Assert
            result.Should().BeTrue();
            _exchangeRateRepositoryMock.Verify(r => r.AddCurrencyPairAsync(It.IsAny<ExchangeRate>()), Times.Once);
        }

        [Fact]
        public async Task AddRateAsync_ShouldThrowException_WhenCurrencyPairAlreadyExists()
        {
            // Arrange
            var rateDto = new ExchangeRateDTO { BaseCurrency = "USD", QuoteCurrency = "EUR", AskPrice = 1.2m, BidPrice = 1.1m };
            var existingRate = new ExchangeRate { CurrencyPair = "USD-EUR" };

            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(true);
            _exchangeRateRepositoryMock.Setup(r => r.GetByCurrencyPairAsync("USD-EUR")).ReturnsAsync(existingRate);

            // Act
            Func<Task> act = async () => await _exchangeRateService.AddRateAsync(rateDto);

            // Assert
            await act.Should().ThrowAsync<CurrencyPairException>()
                .WithMessage("The currency pair already exists!");
        }

        [Fact]
        public async Task UpdateRateAsync_ShouldThrowException_WhenCurrencyPairNotFound()
        {
            // Arrange
            var rateDto = new ExchangeRateDTO { BaseCurrency = "USD", QuoteCurrency = "EUR", AskPrice = 1.2m, BidPrice = 1.1m };
            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(true);
            _exchangeRateRepositoryMock.Setup(r => r.GetByCurrencyPairAsync("USD-EUR")).ReturnsAsync((ExchangeRate)null);

            // Act
            Func<Task> act = async () => await _exchangeRateService.UpdateRateAsync(rateDto);

            // Assert
            await act.Should().ThrowAsync<CurrencyPairException>()
                .WithMessage("The currency pair was not found!");
        }

        [Fact]
        public async Task UpdateRateAsync_ShouldUpdateExistingRate_WhenValid()
        {
            // Arrange
            var rateDto = new ExchangeRateDTO { BaseCurrency = "USD", QuoteCurrency = "EUR", AskPrice = 1.3m, BidPrice = 1.2m };
            var existingRate = new ExchangeRate { CurrencyPair = "USD-EUR", AskPrice = 1.1m, BidPrice = 1.0m };

            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(true);
            _exchangeRateRepositoryMock.Setup(r => r.GetByCurrencyPairAsync("USD-EUR")).ReturnsAsync(existingRate);

            // Act
            var result = await _exchangeRateService.UpdateRateAsync(rateDto);

            // Assert
            result.Should().BeTrue();
            existingRate.AskPrice.Should().Be(rateDto.AskPrice);
            existingRate.BidPrice.Should().Be(rateDto.BidPrice);
            _exchangeRateRepositoryMock.Verify(r => r.UpdateCurrencyPairAsync(existingRate), Times.Once);
        }

        [Fact]
        public async Task DeleteRateAsync_ShouldThrowException_WhenCurrencyPairIsInvalid()
        {
            // Arrange
            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(false);

            // Act
            Func<Task> act = async () => await _exchangeRateService.DeleteRateAsync("INVALID-PAIR");

            // Assert
            await act.Should().ThrowAsync<CurrencyPairException>()
                .WithMessage("Invalid currency format. Expected format: XXX");
        }

        [Fact]
        public async Task DeleteRateAsync_ShouldReturnTrue_WhenRateDeleted()
        {
            // Arrange
            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(true);
            _exchangeRateRepositoryMock.Setup(r => r.DeleteCurrencyPairAsync("USD-EUR")).ReturnsAsync(true);

            // Act
            var result = await _exchangeRateService.DeleteRateAsync("USD-EUR");

            // Assert
            result.Should().BeTrue();
            _exchangeRateRepositoryMock.Verify(r => r.DeleteCurrencyPairAsync("USD-EUR"), Times.Once);
        }

        [Fact]
        public async Task GetLatestRateAsync_ShouldThrowException_WhenCurrencyPairIsInvalid()
        {
            // Arrange
            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(false);

            // Act
            Func<Task> act = async () => await _exchangeRateService.GetLatestRateAsync("INVALID-PAIR");

            // Assert
            await act.Should().ThrowAsync<CurrencyPairException>()
                .WithMessage("Invalid currency format. Expected format: XXX");
        }

        [Fact]
        public async Task GetLatestRateAsync_ShouldReturnNewRate_WhenRateFetched()
        {
            // Arrange
            var currencyPair = "USD-EUR";
            var newRate = new ExchangeRate { CurrencyPair = currencyPair, AskPrice = 1.3m, BidPrice = 1.2m };

            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(true);
            _providerRepositoryMock.Setup(p => p.GetExchangeRateAsync(currencyPair)).ReturnsAsync(newRate);
            _exchangeRateRepositoryMock.Setup(r => r.GetByCurrencyPairAsync(currencyPair)).ReturnsAsync((ExchangeRate)null);

            // Act
            var result = await _exchangeRateService.GetLatestRateAsync(currencyPair);

            // Assert
            result.Should().BeEquivalentTo(newRate);
            _exchangeRateRepositoryMock.Verify(r => r.AddCurrencyPairAsync(newRate), Times.Once);
        }

        [Fact]
        public async Task GetExchangeRateAsync_ShouldThrowException_WhenCurrencyPairIsInvalid()
        {
            // Arrange
            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(false);

            // Act
            Func<Task> act = async () => await _exchangeRateService.GetExchangeRateAsync("INVALID-PAIR");

            // Assert
            await act.Should().ThrowAsync<CurrencyPairException>()
                .WithMessage("Invalid currency pair. Both 'fromCurrency' and 'toCurrency' must be provided. Valid format example: EUR-BRL");
        }

        [Fact]
        public async Task GetExchangeRateAsync_ShouldReturnExistingRate_WhenFoundInRepository()
        {
            // Arrange
            var currencyPair = "USD-EUR";
            var existingRate = new ExchangeRate { CurrencyPair = currencyPair, AskPrice = 1.3m, BidPrice = 1.2m };

            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(true);
            _exchangeRateRepositoryMock.Setup(r => r.GetByCurrencyPairAsync(currencyPair)).ReturnsAsync(existingRate);

            // Act
            var result = await _exchangeRateService.GetExchangeRateAsync(currencyPair);

            // Assert
            result.Should().BeEquivalentTo(existingRate);
        }

        [Fact]
        public async Task GetExchangeRateAsync_ShouldFetchAndAddRate_WhenNotFoundInRepository()
        {
            // Arrange
            var currencyPair = "USD-EUR";
            var newRate = new ExchangeRate { CurrencyPair = currencyPair, AskPrice = 1.3m, BidPrice = 1.2m };

            _exchangeRateRepositoryMock.Setup(r => r.IsValidCurrencyPair(It.IsAny<string>())).Returns(true);
            _exchangeRateRepositoryMock.Setup(r => r.GetByCurrencyPairAsync(currencyPair)).ReturnsAsync((ExchangeRate)null);
            _providerRepositoryMock.Setup(p => p.GetExchangeRateAsync(currencyPair)).ReturnsAsync(newRate);

            // Act
            var result = await _exchangeRateService.GetExchangeRateAsync(currencyPair);

            // Assert
            result.Should().BeEquivalentTo(newRate);
            _exchangeRateRepositoryMock.Verify(r => r.AddCurrencyPairAsync(newRate), Times.Once);
        }

    }
}