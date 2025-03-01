using System.ComponentModel.DataAnnotations;

namespace ForeignExchange.Application.DTOs
{
    public class ExchangeRateDTO
    {
        [Required(ErrorMessage = "Base currency is required.")]
        public string BaseCurrency { get; set; }

        [Required(ErrorMessage = "Quote currency is required.")]
        public string QuoteCurrency { get; set; }

        [Required(ErrorMessage = "Bid price is required.")]
        public decimal BidPrice { get; set; }

        [Required(ErrorMessage = "Ask price is required.")]
        public decimal AskPrice { get; set; }
        [Required(ErrorMessage = "Time stamp is required.")]
        public DateTime Timestamp { get; set; }
    }

}
