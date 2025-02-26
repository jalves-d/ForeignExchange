using ForeignExchange.Domain.Entities;
using ForeignExchange.Domain.Exceptions;
using ForeignExchange.Infrastructure.Interfaces;
using ForeignExchange.Infrastructure.Model;
using Microsoft.AspNet.SignalR.Client.Http;
using System.Net.Http;
using System.Text.Json;

public class AlphaVantageRepository : IForexProviderRepository
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClient;

    public AlphaVantageRepository(IConfiguration configuration, IHttpClientFactory httpClient)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<ExchangeRate?> GetExchangeRateAsync(string currencyPair)
    {
        var currencies = currencyPair.Split('-');
        var fromCurrency = currencies[0];
        var toCurrency = currencies[1];

        var url = string.Format(_configuration["AlphaVantage:BaseUrl"], fromCurrency, toCurrency, _configuration["AlphaVantage:ApiKey"]);
        var client = _httpClient.CreateClient();

        try
        {
            var response = await client.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<ExchangeRateResponse>(response);

            if (data?.RealtimeCurrencyExchangeRate == null)
            {
                throw new CurrencyPairException("Exchange pair doesn't exist or is not listed!");
            }
            if (string.IsNullOrEmpty(data.RealtimeCurrencyExchangeRate.AskPrice) || string.IsNullOrEmpty(data.RealtimeCurrencyExchangeRate.BidPrice))
            {
                throw new ForexProviderException("AskPrice or BidPrice is null or empty.");
            }
            if (decimal.TryParse(data.RealtimeCurrencyExchangeRate.AskPrice, out decimal askPrice) &&
                    decimal.TryParse(data.RealtimeCurrencyExchangeRate.BidPrice, out decimal bidPrice))
            {
                return new ExchangeRate
                {
                    CurrencyPair = $"{fromCurrency}/{toCurrency}",
                    AskPrice = Math.Round(askPrice, 2),
                    BidPrice = Math.Round(bidPrice, 2),
                    UpdatedAt = DateTime.UtcNow
                };
            }
            else
            {
                throw new ForexProviderException("Invalid values to AskPrice or BidPrice.");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new ForexProviderException("Error fetching currency pair information, try again later!", ex);
        }
        catch (JsonException ex)
        {
            throw new Exception("Error parsing exchange rate response.", ex);
        }
    }
}
