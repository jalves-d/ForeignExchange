using FluentValidation;
using ForeignExchange.Application.DTOs;

namespace ForeignExchange.Application.Validations
{
    public class UserValidator : AbstractValidator<UserDTO>
    {
        public UserValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("The username field is mandatory!")
                .MinimumLength(6).WithMessage("Not a valid username!");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("The email field is mandatory!")
                .MinimumLength(6).WithMessage("Not a valid email!");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("The password field is mandatory!")
                .MinimumLength(6).WithMessage("Not a valid password!");
        }
    }
}
