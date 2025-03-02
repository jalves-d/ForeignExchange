using FluentValidation;

namespace ForeignExchange.Application.Validations
{
    public class StringValidator : AbstractValidator<string>
    {
        public StringValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("The input fields can't be empty.")
                .MinimumLength(3).WithMessage("The input lenght should be bigger then 3.");
        }
    }
}
