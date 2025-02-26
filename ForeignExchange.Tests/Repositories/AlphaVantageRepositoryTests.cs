using ForeignExchange.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using Xunit;

namespace ForeignExchange.Tests.Repositories
{
    public class AlphaVantageRepositoryTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly AlphaVantageRepository _repository;

        public AlphaVantageRepositoryTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _configurationMock.Setup(c => c["AlphaVantage:BaseUrl"]).Returns("https://www.alphavantage.co/query?function=FX_REALTIME&from_currency={0}&to_currency={1}&apikey={2}");
            _configurationMock.Setup(c => c["AlphaVantage:ApiKey"]).Returns("41O60PVJPCW4F6FD");

            _repository = new AlphaVantageRepository(_configurationMock.Object, new HttpClientFactory(_httpClient));
        }

        [Fact]
        public async Task GetExchangeRateAsync_ShouldReturnExchangeRate_WhenValidCurrencyPair()
        {
            // Arrange
            var currencyPair = "USD-JPY";
            var jsonResponse = "{\"Realtime Currency Exchange Rate\": {\"1. From_Currency Code\": \"USD\",\"2. From_Currency Name\": \"United States Dollar\",\"3. To_Currency Code\": \"JPY\",\"4. To_Currency Name\": \"Japanese Yen\",\"5. Exchange Rate\": \"149.13200000\",\"6. Last Refreshed\": \"2025-02-25 18:40:36\",\"7. Time Zone\": \"UTC\",\"8. Bid Price\": \"149.12980000\",\"9. Ask Price\": \"149.13630000\"}}";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse),
                });

            // Act
            var result = await _repository.GetExchangeRateAsync(currencyPair);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("USD/JPY", result.CurrencyPair);
        }

        [Fact]
        public async Task GetExchangeRateAsync_ShouldThrowCurrencyPairException_WhenInvalidCurrencyPair()
        {
            var currencyPair = "INVALID-PAIR";
            var jsonResponse = "{\"Realtime Currency Exchange Rate\": null}";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse),
                });

            await Assert.ThrowsAsync<CurrencyPairException>(() => _repository.GetExchangeRateAsync(currencyPair));
        }

        [Fact]
        public async Task GetExchangeRateAsync_ShouldThrowForexProviderException_WhenHttpRequestFails()
        {
            var currencyPair = "USD-EUR";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            var exception = await Assert.ThrowsAsync<ForexProviderException>(() => _repository.GetExchangeRateAsync(currencyPair));
            Assert.Equal("Error fetching currency pair information, try again later!", exception.Message);
        }

        [Fact]
        public async Task GetExchangeRateAsync_ShouldThrowException_WhenJsonParsingFails()
        {
            var currencyPair = "USD-EUR";
            var invalidJsonResponse = "invalid json response";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(invalidJsonResponse),
                });

            var exception = await Assert.ThrowsAsync<Exception>(() => _repository.GetExchangeRateAsync(currencyPair));
            Assert.Equal("Error parsing exchange rate response.", exception.Message);
        }
    }
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public HttpClientFactory(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClient CreateClient(string name)
        {
            return _httpClient;
        }
    }
}