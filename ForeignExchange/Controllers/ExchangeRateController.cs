using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Domain.Exceptions;
using ForeignExchange.Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ForeignExchange.Controllers
{
    [ApiController]
    [Route("api/exchange-rates")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeRateController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }


        /// <summary>
        /// Get all exchange rates
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves all available exchange rates from the database.
        /// </remarks>
        /// <returns>A list of exchange rates.</returns>
        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Get all exchange rates", OperationId = "GetAllExchangeRates")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var rates = await _exchangeRateService.GetAllRatesAsync();
                return Ok(rates);
            }
            catch (Exception)
            {
                return NotFound("Error retrieving Data!");
            }
        }

        /// <summary>
        /// Get exchange rate for a specific currency pair
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves the exchange rate for a specified currency pair. 
        /// If the rate is not found in the database, it will attempt to fetch it.
        /// </remarks>
        /// <param name="baseCurrency">The base currency code.</param>
        /// <param name="quoteCurrency">The quote currency code.</param>
        /// <returns>The exchange rate for the specified currency pair.</returns>
        [HttpGet("{baseCurrency}/{quoteCurrency}")]
        [Authorize]
        [SwaggerOperation(Summary = "Get exchange rate for a specific currency pairs", OperationId = "GetRate")]
        public async Task<IActionResult> GetRate(string baseCurrency, string quoteCurrency)
        {
            try
            {
                var rate = await _exchangeRateService.GetExchangeRateAsync(baseCurrency + "-" + quoteCurrency);
                return Ok(rate);
            }
            catch (Exception)
            {
                return NotFound("Error retrieving Data!");
            }
        }

        /// <summary>
        /// Create a new exchange rate
        /// </summary>
        /// <remarks>
        /// This endpoint allows the creation of a new exchange rate.
        /// </remarks>
        /// <param name="rateDto">The exchange rate data transfer object.</param>
        /// <returns>Confirmation of the created exchange rate.</returns>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Create a new exchange rate", OperationId = "CreateNewRate")]
        public async Task<IActionResult> CreateNewRate([FromBody] ExchangeRateDTO rateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _exchangeRateService.AddRateAsync(rateDto);
                return Ok("Currency pair " + rateDto.BaseCurrency + "/" + rateDto.QuoteCurrency + " was created!");
            }
            catch (Exception ex)
            {
                return NotFound("Creation not completed due to error: " + ex.Message);
            }
        }

        /// <summary>
        /// Update an existing exchange rate
        /// </summary>
        /// <remarks>
        /// This endpoint updates an existing exchange rate with new data.
        /// </remarks>
        /// <param name="rateDto">The exchange rate data transfer object.</param>
        /// <returns>Confirmation of the updated exchange rate.</returns>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(Summary = "Update an existing exchange rate", OperationId = "UpdateRate")]
        public async Task<IActionResult> UpdateRate([FromBody] ExchangeRateDTO rateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _exchangeRateService.UpdateRateAsync(rateDto);
                return Ok("Currency pair " + rateDto.BaseCurrency + "/" + rateDto.QuoteCurrency + " was updated!");
            }
            catch (Exception ex)
            {
                return NotFound("Update not completed due to error: " + ex.Message);
            }
        }

        /// <summary>
        /// Delete an exchange rate
        /// </summary>
        /// <remarks>
        /// This endpoint deletes an exchange rate for the specified currency pair.
        /// </remarks>
        /// <param name="baseCurrency">The base currency code.</param>
        /// <param name="quoteCurrency">The quote currency code.</param>
        /// <returns>Confirmation of the deleted exchange rate.</returns>
        [HttpDelete("{baseCurrency}/{quoteCurrency}")]
        [Authorize]
        [SwaggerOperation(Summary = "Delete an exchange rate", OperationId = "DeleteRate")]
        public async Task<IActionResult> DeleteRate(string baseCurrency, string quoteCurrency)
        {
            try
            {
                var success = await _exchangeRateService.DeleteRateAsync(baseCurrency + "-" + quoteCurrency);
                return Ok("Currency pair " + baseCurrency + "/" + quoteCurrency + " was deleted!");
            }
            catch (Exception ex)
            {
                return NotFound("Currency pair not found due to error: " + ex.Message);
            }
        }

        /// <summary>
        /// Get the latest exchange rate
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves the latest exchange rate for a specific currency pair.
        /// </remarks>
        /// <param name="baseCurrency">The base currency code.</param>
        /// <param name="quoteCurrency">The quote currency code.</param>
        /// <returns>The latest exchange rate for the specified currency pair.</returns>
        [HttpGet("latest/{baseCurrency}/{quoteCurrency}")]
        [Authorize]
        [SwaggerOperation(Summary = "Get the latest exchange rate", OperationId = "GetLatestRate")]
        public async Task<IActionResult> GetLatestRate(string baseCurrency, string quoteCurrency)
        {
            try
            {
                var newRate = await _exchangeRateService.GetLatestRateAsync(baseCurrency + "-" + quoteCurrency);
                return Ok(newRate);
            }
            catch (Exception)
            {
                return NotFound("Error retrieving Data!");
            }
        }

        /// <summary>
        /// Get exchange rate by currency pair
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves the exchange rate for a given currency pair.
        /// </remarks>
        /// <param name="currencyPair">The currency pair in the format 'BaseCurrency-QuoteCurrency'.</param>
        /// <returns>The exchange rate for the specified currency pair.</returns>
        [HttpGet("{currencyPair}")]
        [Authorize]
        [SwaggerOperation(Summary = "Get exchange rate by currency pair", OperationId = "GetRateByPair")]
        public async Task<IActionResult> GetRateByPair(string currencyPair)
        {
            try
            {
                var rate = await _exchangeRateService.GetExchangeRateAsync(currencyPair);
                return Ok(rate);
            }
            catch (Exception)
            {
                return NotFound("Error retrieving Data!");
            }
        }
    }
}
