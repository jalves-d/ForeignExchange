using FluentValidation;
using ForeignExchange.Application.DTOs;

namespace ForeignExchange.Application.Validations
{
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(u => u.Login)
                .NotEmpty().WithMessage("The login field is mandatory!")
                .MinimumLength(6).WithMessage("Not a valid username or email!");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("The password field is mandatory!")
                .MinimumLength(6).WithMessage("Not a valid password!");
        }
    }
}
