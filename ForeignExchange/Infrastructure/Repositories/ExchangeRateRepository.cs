using ForeignExchange.Application.Services;
using ForeignExchange.Domain.Entities;
using ForeignExchange.Domain.Exceptions;
using ForeignExchange.Infrastructure.Data;
using ForeignExchange.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExchangeRateRepository> _logger;

    #region consts
    private static readonly Regex CurrencyPairRegex = new Regex(@"^[A-Z]{3}-[A-Z]{3}$", RegexOptions.Compiled);
    #endregion

    public ExchangeRateRepository(AppDbContext context, IConfiguration configuration, ILogger<ExchangeRateRepository> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ExchangeRate?> GetByCurrencyPairAsync(string currencyPair)
    {
        var exchangeRate = await _context.ExchangeRates
            .FirstOrDefaultAsync(r => r.CurrencyPair == currencyPair.Replace('-', '/'));
                
        if (exchangeRate != null)
        {
            _logger.LogInformation("Currency pair " + exchangeRate.CurrencyPair + " Last Update was in " + exchangeRate.UpdatedAt.ToString());
            return exchangeRate;
        }

        return null;
    }

    public bool IsValidCurrencyPair(string currencyPair)
    {
        if (string.IsNullOrWhiteSpace(currencyPair))
        {
            return false;
        }

        _logger.LogInformation("Currency pair " + currencyPair + " Validated!");

        return CurrencyPairRegex.IsMatch(currencyPair);
    }

    public async Task<ExchangeRate?> AddCurrencyPairAsync(ExchangeRate exchangeRate)
    {
        exchangeRate.CurrencyPair = exchangeRate.CurrencyPair.Replace('-', '/');
        _context.ExchangeRates.Add(exchangeRate);
        await _context.SaveChangesAsync();


        _logger.LogInformation("Currency pair " + exchangeRate.CurrencyPair + " information saved in the database in " + exchangeRate.UpdatedAt.ToString());

        return exchangeRate;
    }

    public async Task<ExchangeRate?> UpdateCurrencyPairAsync(ExchangeRate exchangeRate)
    {
        _context.ExchangeRates.Update(exchangeRate);
        await _context.SaveChangesAsync();


        _logger.LogInformation("Currency pair " + exchangeRate.CurrencyPair + " new update in " + exchangeRate.UpdatedAt.ToString());

        return exchangeRate;
    }

    public async Task<bool> DeleteCurrencyPairAsync(string currencyPair)
    {
        var exchangeRate = await _context.ExchangeRates
            .FirstOrDefaultAsync(r => r.CurrencyPair == currencyPair.Replace('-', '/'));

        try
        {
            _context.ExchangeRates.Remove(exchangeRate);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Currency pair " + currencyPair + " deleted in " + exchangeRate.UpdatedAt.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError("Error trying to delele the currency pair " + currencyPair + " !");
            throw new CurrencyPairException("Invalid currency pair or the currency pair is not registered!");
        }
        return true;
    }

    public async Task<IEnumerable<ExchangeRate>> GetAllCurrencyPairsAsync()
    {
        var exchangeRateList = await _context.ExchangeRates.ToListAsync();

        return exchangeRateList ?? new List<ExchangeRate>();
    }
}