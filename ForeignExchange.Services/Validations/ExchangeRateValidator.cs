using FluentValidation;
using ForeignExchange.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForeignExchange.Application.Validations
{
    public class ExchangeRateValidator : AbstractValidator<ExchangeRateDTO>
    {
        public ExchangeRateValidator()
        {
            RuleFor(e => e.BaseCurrency)
                .NotEmpty().WithMessage("The base currency field is mandatory.")
                .MinimumLength(3).WithMessage("Not a valid username.");

            RuleFor(e => e.QuoteCurrency)
                .NotEmpty().WithMessage("The quote currency field is mandatory.")
                .MinimumLength(3).WithMessage("Not a valid username.");

            RuleFor(e => e.AskPrice)
                .NotEmpty().WithMessage("The ask price field is mandatory.")
                .GreaterThan(0).WithMessage("The ask price should be greather than 0.");

            RuleFor(e => e.BidPrice)
                .NotEmpty().WithMessage("The bid price field is mandatory.")
                .GreaterThan(0).WithMessage("The bid price should be greather than 0.");

            RuleFor(e => e.Timestamp)
                .NotEmpty().WithMessage("The timestamp field is mandatory.");
        }
    }
}
