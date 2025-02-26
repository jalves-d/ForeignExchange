using ForeignExchange.Domain.Entities;
using ForeignExchange.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ForeignExchange.Tests.Repositories
{
    public class ExchangeRateRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ExchangeRateRepository _repository;

        public ExchangeRateRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new ExchangeRateRepository(_context, Mock.Of<IConfiguration>(), Mock.Of<ILogger<ExchangeRateRepository>>());
        }

        [Fact]
        public async Task GetByCurrencyPairAsync_ShouldReturnExchangeRate_WhenExists()
        {
            // Arrange
            var exchangeRate = new ExchangeRate { CurrencyPair = "USD/EUR", AskPrice = 1.10m, BidPrice = 1.09m, UpdatedAt = DateTime.UtcNow };
            _context.ExchangeRates.Add(exchangeRate);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCurrencyPairAsync("USD/EUR");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("USD/EUR", result.CurrencyPair);
        }

        [Fact]
        public async Task GetByCurrencyPairAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByCurrencyPairAsync("USD/EUR");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void IsValidCurrencyPair_ShouldReturnTrue_WhenValid()
        {
            // Act
            var result = _repository.IsValidCurrencyPair("USD-EUR");

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("USD EUR")]
        [InlineData("USDEUR")]
        public void IsValidCurrencyPair_ShouldReturnFalse_WhenInvalid(string currencyPair)
        {
            // Act
            var result = _repository.IsValidCurrencyPair(currencyPair);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddCurrencyPairAsync_ShouldAddAndReturnExchangeRate()
        {
            // Arrange
            var exchangeRate = new ExchangeRate { CurrencyPair = "USD/EUR", AskPrice = 1.10m, BidPrice = 1.09m, UpdatedAt = DateTime.UtcNow };

            // Act
            var result = await _repository.AddCurrencyPairAsync(exchangeRate);
            var dbResult = await _context.ExchangeRates.FirstOrDefaultAsync(r => r.CurrencyPair == "USD/EUR");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(dbResult);
            Assert.Equal(1.10m, dbResult.AskPrice);
        }

        [Fact]
        public async Task UpdateCurrencyPairAsync_ShouldUpdateAndReturnExchangeRate()
        {
            // Arrange
            var exchangeRate = new ExchangeRate { CurrencyPair = "USD/EUR", AskPrice = 1.10m, BidPrice = 1.09m, UpdatedAt = DateTime.UtcNow };
            _context.ExchangeRates.Add(exchangeRate);
            await _context.SaveChangesAsync();

            // Modifica o preço
            exchangeRate.AskPrice = 1.11m;

            // Act
            var result = await _repository.UpdateCurrencyPairAsync(exchangeRate);
            var dbResult = await _context.ExchangeRates.FirstOrDefaultAsync(r => r.CurrencyPair == "USD/EUR");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(dbResult);
            Assert.Equal(1.11m, dbResult.AskPrice);
        }

        [Fact]
        public async Task DeleteCurrencyPairAsync_ShouldDeleteExchangeRate()
        {
            // Arrange
            var exchangeRate = new ExchangeRate { CurrencyPair = "USD/EUR", AskPrice = 1.10m, BidPrice = 1.09m, UpdatedAt = DateTime.UtcNow };
            _context.ExchangeRates.Add(exchangeRate);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteCurrencyPairAsync("USD/EUR");
            var dbResult = await _context.ExchangeRates.FirstOrDefaultAsync(r => r.CurrencyPair == "USD/EUR");

            // Assert
            Assert.True(result);
            Assert.Null(dbResult);
        }

        [Fact]
        public async Task GetAllCurrencyPairsAsync_ShouldReturnAllExchangeRates()
        {
            // Arrange
            _context.ExchangeRates.Add(new ExchangeRate { CurrencyPair = "USD/EUR", AskPrice = 1.10m, BidPrice = 1.09m, UpdatedAt = DateTime.UtcNow });
            _context.ExchangeRates.Add(new ExchangeRate { CurrencyPair = "EUR/JPY", AskPrice = 1.20m, BidPrice = 1.19m, UpdatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllCurrencyPairsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
