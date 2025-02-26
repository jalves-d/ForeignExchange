using System.ComponentModel.DataAnnotations;

namespace ForeignExchange.Domain.Entities
{
    public class ExchangeRate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CurrencyPair { get; set; } // "USD/EUR"
        [Required]
        public decimal BidPrice { get; set; }
        [Required]
        public decimal AskPrice { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
